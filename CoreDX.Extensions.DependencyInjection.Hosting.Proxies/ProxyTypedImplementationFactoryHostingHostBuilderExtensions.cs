using CoreDX.Extensions.DependencyInjection;
using CoreDX.Extensions.DependencyInjection.Hosting.Proxies;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class ProxyTypedImplementationFactoryHostingHostBuilderExtensions
{
    /// <summary>
    /// Specify the <see cref="IServiceProvider"/> to be the dynamic proxy and typed implementation factory supported one.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder UseProxyTypedImplementationFactoryServiceProvider(
        this IHostBuilder hostBuilder)
        => hostBuilder.UseProxyTypedImplementationFactoryServiceProvider(static _ => { });

    /// <summary>
    /// Specify the <see cref="IServiceProvider"/> to be the dynamic proxy and typed implementation factory supported one.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <param name="configure">The delegate that configures the <see cref="IServiceProvider"/>.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder UseProxyTypedImplementationFactoryServiceProvider(
        this IHostBuilder hostBuilder,
        Action<ServiceProviderOptions> configure)
        => hostBuilder.UseProxyTypedImplementationFactoryServiceProvider((context, options) => configure(options));

    /// <summary>
    /// Specify the <see cref="IServiceProvider"/> to be the dynamic proxy and typed implementation factory supported one.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <param name="configure">The delegate that configures the <see cref="IServiceProvider"/>.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder UseProxyTypedImplementationFactoryServiceProvider(
        this IHostBuilder hostBuilder,
        Action<HostBuilderContext, ServiceProviderOptions> configure)
    {
        return hostBuilder.UseServiceProviderFactory(context =>
        {
            var options = new ServiceProviderOptions();
            configure(context, options);
            return new ProxyTypedImplementationFactoryServiceProviderFactory(options);
        });
    }
}
