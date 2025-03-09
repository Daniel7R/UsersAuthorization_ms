using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using UsersAuthorization.Application.Interfaces;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Repository;
using Microsoft.Extensions.Options;
using UsersAuthorization.Application.Queues;
using UsersAuthorization.Application.Messages.Response;
using UsersAuthorization.Application.EventHandler;

//consumer
namespace UsersAuthorization.Infrastructure.EventBus
{
    public class EventBusConsumer: EventBusBase, IEventBusConsumer
    {
        //private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMQSettings _rabbitmqSettings;
        private readonly Dictionary<string, Func<string, Task<string>>> _handlers;
        private readonly ILogger<EventBusConsumer> _logger;

        public EventBusConsumer(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options, ILogger<EventBusConsumer> logger): base(options)
        {
            _rabbitmqSettings = options.Value;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new();
            _logger = logger;

            InitializeAsync().GetAwaiter().GetResult();
        }

        public static async Task<EventBusConsumer> CreateAsync(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options, ILogger<EventBusConsumer> logger)
        {
            var instance = new EventBusConsumer(serviceScopeFactory, options, logger);
            await instance.InitializeAsync();
            return instance;
        }
        private async Task InitializeAsync()
        {
            await base.InitializeAsync();

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            RegisterQueueHandler<int, GetUserByIdResponse>(Queues.GET_USER_BY_ID, async (idUser) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetRequiredService<UserEventHandler>();

                    var responseHandler = await handler.GetUserInfo(idUser);

                    return responseHandler;
                }
            });

            RegisterQueueHandler<List<int>, List<GetUserByIdResponse>>(Queues.USERS_BULK_INFO, async (idUsers) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetRequiredService<UserEventHandler>();

                    var responseHandler = await handler.GetUsersInfo(idUsers);

                    return responseHandler;
                }
            });

            RegisterQueueHandler<object?, List<int>>(Queues.ALL_USER_EMAILS, async (obj) =>
            {
                using(var scope = _serviceScopeFactory.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetRequiredService<UserEventHandler>();

                    var responseHandler = await handler.GetAllUserEmails();

                    return responseHandler;
                }
            });
        }

        public void RegisterQueueHandler<TRequest, TResponse>(string queueName, Func<TRequest, Task<TResponse>> handler)
        {
            if (_channel == null) throw new InvalidOperationException("EventBusRabbitMQ is not initialized.");
            if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen )
            {
                Task.Run(InitializeAsync).GetAwaiter().GetResult();
            }

            _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, null).Wait();
            _channel.BasicQosAsync(0, 1, false);

            _handlers[queueName] = async (message) =>
            {
                var request = JsonConvert.DeserializeObject<TRequest>(message);
                var response = await handler(request);
                return JsonConvert.SerializeObject(response);
            };

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var replyProps = new BasicProperties
                {
                    CorrelationId = ea.BasicProperties.CorrelationId
                };

                try
                {
                    if (_handlers.TryGetValue(ea.RoutingKey, out var handler))
                    {
                        var responseMessage = await handler(message);
                        var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                        await _channel.BasicPublishAsync(exchange: "", routingKey: ea.BasicProperties.ReplyTo, mandatory: false, basicProperties: replyProps, body: responseBytes);
                    }
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex) {
                    _logger.LogError($"Error handling queue req {ex.Message}");
                    //to reject in fail case
                    if (!_connection.IsOpen)
                    {
                        await InitializeAsync();
                    }
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer).Wait();
        }
    }
}
