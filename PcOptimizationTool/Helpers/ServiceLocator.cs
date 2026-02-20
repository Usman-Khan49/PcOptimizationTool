namespace PcOptimizationTool.Helpers
{
    /// <summary>
    /// Simple service locator for dependency injection
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator? _instance;
        private readonly Dictionary<Type, object> _services = new();

        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        public void RegisterService<TInterface, TImplementation>(TImplementation implementation)
            where TImplementation : TInterface
        {
            _services[typeof(TInterface)] = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        public void RegisterService<T>(T implementation)
        {
            _services[typeof(T)] = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} not registered");
        }

        public T? TryGetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            return default;
        }
    }
}
