using Microsoft.Extensions.DependencyInjection;

namespace CoreDX.Extensions.DependencyInjection;

internal sealed class ImplementationFactoryServiceTypeHolder(Type serviceType)
{
    private readonly Func<IServiceProvider, object?> _factory = sp => sp.GetService(serviceType);

    public Func<IServiceProvider, object?> Factory => _factory;
}

internal sealed class KeyedImplementationFactoryServiceTypeHolder(Type serviceType)
{
    private readonly Func<IServiceProvider, object?, object?> _factory = (sp, key) => (sp as IKeyedServiceProvider)?.GetKeyedService(serviceType, key);

    public Func<IServiceProvider, object?, object?> Factory => _factory;
}

internal sealed class OpenGenericImplementationFactoryServiceTypeHolder(Type serviceType)
{
    private readonly Func<IServiceProvider, Type, object?> _factory = serviceType.IsGenericTypeDefinition
        ? (sp, type) =>
        {
            var closed = serviceType.MakeGenericType(type.GenericTypeArguments);
            return sp.GetService(closed);
        }
        : throw new ArgumentException($"{nameof(serviceType)} is not generic type definition.");

    public Func<IServiceProvider, Type, object?> Factory => _factory;
}

internal sealed class KeyedOpenGenericImplementationFactoryServiceTypeHolder(Type serviceType)
{
    private readonly Func<IServiceProvider, object?, Type, object?> _factory = serviceType.IsGenericTypeDefinition
        ? (sp, key, type) =>
        {
            var closed = serviceType.MakeGenericType(type.GenericTypeArguments);
            return (sp as IKeyedServiceProvider)?.GetKeyedService(closed, key);
        }
        : throw new ArgumentException($"{nameof(serviceType)} is not generic type definition.");

    public Func<IServiceProvider, object?, Type, object?> Factory => _factory;
}