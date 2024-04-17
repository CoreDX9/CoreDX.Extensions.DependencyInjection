using Microsoft.Extensions.DependencyInjection;

namespace CoreDX.Extensions.DependencyInjection.Hosting.Proxies;

/// <summary>
/// Proxy typed implementation factory supported implementation of <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
/// </summary>
public class ProxyTypedImplementationFactoryServiceProviderFactory : TypedImplementationFactoryServiceProviderFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyTypedImplementationFactoryServiceProviderFactory"/> class
    /// with default options.
    /// </summary>
    public ProxyTypedImplementationFactoryServiceProviderFactory() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedImplementationFactoryServiceProviderFactory"/> class
    /// with default options.
    /// </summary>
    /// <param name="options">The options to use for this instance.</param>
    public ProxyTypedImplementationFactoryServiceProviderFactory(ServiceProviderOptions options) : base(options) { }

    /// <inheritdoc />
    public override IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        containerBuilder.SolidifyOpenGenericServiceProxyRegister();

        return base.CreateServiceProvider(containerBuilder);
    }
}
