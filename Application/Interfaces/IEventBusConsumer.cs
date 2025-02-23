namespace UsersAuthorization.Application.Interfaces
{
    public interface IEventBusConsumer
    {
        //Task<TResponse> SendRequestAsync<TResquest, TResponse>(TResquest resquest, string queueName);
        void RegisterQueueHandler<TRequest, TResponse>(string queueName, Func<TRequest, Task<TResponse>> handler);
    }
}
