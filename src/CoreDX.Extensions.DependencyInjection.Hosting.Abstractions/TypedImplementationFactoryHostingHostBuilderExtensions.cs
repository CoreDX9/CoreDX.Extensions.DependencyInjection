using CoreDX.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to configure service provider factory of <see cref="IHostBuilder"/>.
/// </summary>
public static class TypedImplementationFactoryHostingHostBuilderExtensions
{
    /// <summary>
    /// Specify the <see cref="IServiceProvider"/> to be the typed implementation factory supported one.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder UseTypedImplementationFactoryServiceProvider(
        this IHostBuilder hostBuilder)
        => hostBuilder.UseTypedImplementationFactoryServiceProvider(static _ => { });

    /// <summary>
    /// Specify the <see cref="IServiceProvider"/> to be the typed implementation factory supported one.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <param name="configure">The delegate that configures the <see cref="IServiceProvider"/>.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder UseTypedImplementationFactoryServiceProvider(
        this IHostBuilder hostBuilder,
        Action<ServiceProviderOptions> configure)
        => hostBuilder.UseTypedImplementationFactoryServiceProvider((context, options) => configure(options));

    /// <summary>
    /// Specify the <see cref="IServiceProvider"/> to be the typed implementation factory supported one.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <param name="configure">The delegate that configures the <see cref="IServiceProvider"/>.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder UseTypedImplementationFactoryServiceProvider(
        this IHostBuilder hostBuilder,
        Action<HostBuilderContext, ServiceProviderOptions> configure)
    {
        return hostBuilder.UseServiceProviderFactory(context =>
        {
            var options = new ServiceProviderOptions();
            configure(context, options);
            return new TypedImplementationFactoryServiceProviderFactory(options);
        });
    }
}
