namespace CoreDX.Extensions.DependencyInjection.ServiceLookup;

internal sealed class TypedServiceImplementationFactoryHolder
{
    private readonly Func<IServiceProvider, Type, object> _factory;
    private readonly Type _serviceType;

    internal TypedServiceImplementationFactoryHolder(Func<IServiceProvider, Type, object> factory, Type serviceType)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
    }

    internal Func<IServiceProvider, object> Factory => FactoryFunc;

    private object FactoryFunc(IServiceProvider provider)
    {
        return _factory(provider, _serviceType);
    }
}

internal sealed class TypedKeyedServiceImplementationFactoryHolder
{
    private readonly Func<IServiceProvider, object?, Type, object> _factory;
    private readonly Type _serviceType;

    internal TypedKeyedServiceImplementationFactoryHolder(Func<IServiceProvider, object?, Type, object> factory, Type serviceType)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
    }

    internal Func<IServiceProvider, object?, object> Factory => FactoryFunc;

    private object FactoryFunc(IServiceProvider provider, object? serviceKey)
    {
        return _factory(provider, serviceKey, _serviceType);
    }
}
