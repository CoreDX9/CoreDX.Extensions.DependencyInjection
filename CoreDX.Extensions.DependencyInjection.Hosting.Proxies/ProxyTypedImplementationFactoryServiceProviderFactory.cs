using Microsoft.Extensions.DependencyInjection;

namespace CoreDX.Extensions.DependencyInjection.Hosting.Proxies;

public class ProxyTypedImplementationFactoryServiceProviderFactory : TypedImplementationFactoryServiceProviderFactory
{
    public ProxyTypedImplementationFactoryServiceProviderFactory() : base() { }

    public ProxyTypedImplementationFactoryServiceProviderFactory(ServiceProviderOptions options) : base(options) { }

    public override IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        containerBuilder.SolidifyOpenGenericServiceProxyRegister();

        return base.CreateServiceProvider(containerBuilder);
    }
}
