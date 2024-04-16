namespace CoreDX.Extensions.DependencyInjection.Proxies;

/// <summary>
/// The interface for get explicit proxy service. 
/// </summary>
/// <typeparam name="TService">The type of original service to get explicit proxy.</typeparam>
public interface IProxyService<out TService>
    where TService : class
{
    /// <summary>
    /// Get proxy service instance of type <typeparamref name="TService"/>.
    /// </summary>
    TService Proxy { get; }
}

/// <summary>
/// The type for get explicit proxy service. 
/// </summary>
/// <typeparam name="TService">The type of original service to get explicit proxy.</typeparam>
/// <param name="service">Object instance of original service to be proxy.</param>
internal sealed class ProxyService<TService>(TService service) : IProxyService<TService>
    where TService : class
{
    public TService Proxy { get; } = service;
}
