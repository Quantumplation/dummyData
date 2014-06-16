namespace DummyDataSample
{
    public interface IServiceProvider
    {
        T Get<T>() where T : Service, new();
    }
    public class ServiceProvider : IServiceProvider
    {
        public T Get<T>() where T : Service, new()
        {
            return new T()
            {
                ServiceProvider = this
            };
        }
    }
}
