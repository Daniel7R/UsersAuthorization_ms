using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using UsersAuthorization.Application.Interfaces;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Repository;
using Microsoft.Extensions.Options;
using UsersAuthorization.Domain.Messages;

//consumer
namespace UsersAuthorization.Infrastructure.EventBus
{
    public class EventBusConsumer: BackgroundService, IEventBusConsumer, IAsyncDisposable
    {
        private IConnection _connection; 
        private IChannel _channel;
        //private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMQSettings _rabbitmqSettings;
        private readonly Dictionary<string, Func<string, Task<string>>> _handlers;

        public EventBusConsumer(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options)
        {
            Console.Write("trying to connect");
            _rabbitmqSettings = options.Value;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new();
        }

        public static async Task<EventBusConsumer> CreateAsync(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMQSettings> options)
        {
            var instance = new EventBusConsumer(serviceScopeFactory, options);
            await instance.InitializeAsync();
            return instance;
        }
        private async Task InitializeAsync()
        {

            var factory = new ConnectionFactory {
                HostName = _rabbitmqSettings.Host,
                UserName = _rabbitmqSettings.Username,
                Password = _rabbitmqSettings.Password,
                Port = _rabbitmqSettings.Port,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
                RequestedHeartbeat = TimeSpan.FromSeconds(30),
                ContinuationTimeout = TimeSpan.FromSeconds(30),
            };
            while (_connection == null || !_connection.IsOpen || _channel == null || _channel.IsClosed)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();
                }
                catch (Exception ex)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            RegisterQueueHandler<GetUserByIdRequest, GetUserByIdResponse>("GetUserById", async (request) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<ApplicationUser>>();
                    var user = await userRepository.GetByIdAsync(request.Id);
                    return new GetUserByIdResponse
                    {
                        Id = user?.Id ?? 0,
                        Name = user?.Name,
                        Email = user?.Email
                    };
                }
            
                /*var user = await _userRepository.GetByIdAsync(request.Id);
                return new GetUserByIdResponse
                {
                    Id = user?.Id ?? 0,
                    Name = user?.Name,
                    Email = user?.Email
                };*/
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

        /*
        public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest request, string queueName)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                await InitializeAsync();
            }

            var replyQueueName = (await _channel.QueueDeclareAsync()).QueueName;
            var correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = replyQueueName
            };

            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));

            await _channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: false, basicProperties: props, body: messageBytes);

            var tcs = new TaskCompletionSource<TResponse>();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync +=  async (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var response = JsonConvert.DeserializeObject<TResponse>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    tcs.SetResult(response);
                }

                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(consumer: consumer, queue: replyQueueName, autoAck: true);

            return await tcs.Task;
        }
        */


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
                {
                    await InitializeAsync();
                }

                try
                {
                    await Task.Delay(500, stoppingToken);
                }
                catch (Exception ex)
                {
                    await InitializeAsync();
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync(); 
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
