using CoreDX.Extensions.DependencyInjection;
using CoreDX.Extensions.DependencyInjection.Abstractions;

namespace Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to try add service forward.
/// </summary>
public static partial class ServiceCollectionForwardServiceExtensions
{
    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    public static void TryAddForward(
        this IServiceCollection services,
        Type serviceType,
        Type forwardTargetServiceType,
        ServiceLifetime serviceLifetime)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(forwardTargetServiceType);
#else
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (forwardTargetServiceType is null)
        {
            throw new ArgumentNullException(nameof(forwardTargetServiceType));
        }
#endif

        if (serviceType.IsGenericTypeDefinition)
        {
            services.TryAdd(new TypedImplementationFactoryServiceDescriptor(serviceType, new OpenGenericImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }
        else
        {
            services.TryAdd(new ServiceDescriptor(serviceType, new ImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    public static void TryAddForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddForward(typeof(TService), typeof(TForwardTargetService), serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    public static void TryAddKeyedForward(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Type forwardTargetServiceType,
        ServiceLifetime serviceLifetime)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(forwardTargetServiceType);
#else
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (forwardTargetServiceType is null)
        {
            throw new ArgumentNullException(nameof(forwardTargetServiceType));
        }
#endif

        if (serviceType.IsGenericTypeDefinition)
        {
            services.TryAdd(new TypedImplementationFactoryServiceDescriptor(serviceType, serviceKey, new KeyedOpenGenericImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }
        else
        {
            services.TryAdd(new ServiceDescriptor(serviceType, serviceKey, new KeyedImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    public static void TryAddKeyedForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddKeyedForward(typeof(TService), serviceKey, typeof(TForwardTargetService), serviceLifetime);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    public static void TryAddSingletonForward(
        this IServiceCollection services,
        Type serviceType,
        Type forwardTargetServiceType)
    {
        services.TryAddForward(serviceType, forwardTargetServiceType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    public static void TryAddSingletonForward<TService, TForwardTargetService>(this IServiceCollection services)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddSingletonForward(typeof(TService), typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    public static void TryAddKeyedSingletonForward(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Type forwardTargetServiceType)
    {
        services.TryAddKeyedForward(serviceType, serviceKey, forwardTargetServiceType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    public static void TryAddKeyedSingletonForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddKeyedSingletonForward(typeof(TService), serviceKey, typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    public static void TryAddScopedForward(
        this IServiceCollection services,
        Type serviceType,
        Type forwardTargetServiceType)
    {
        services.TryAddForward(serviceType, forwardTargetServiceType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    public static void TryAddScopedForward<TService, TForwardTargetService>(this IServiceCollection services)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddScopedForward(typeof(TService), typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    public static void TryAddKeyedScopedForward(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Type forwardTargetServiceType)
    {
        services.TryAddKeyedForward(serviceType, serviceKey, forwardTargetServiceType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    public static void TryAddKeyedScopedForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddKeyedScopedForward(typeof(TService), serviceKey, typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    public static void TryAddTransientForward(
        this IServiceCollection services,
        Type serviceType,
        Type forwardTargetServiceType)
    {
        services.TryAddForward(serviceType, forwardTargetServiceType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    public static void TryAddTransientForward<TService, TForwardTargetService>(this IServiceCollection services)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddTransientForward(typeof(TService), typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    public static void TryAddKeyedTransientForward(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Type forwardTargetServiceType)
    {
        services.TryAddKeyedForward(serviceType, serviceKey, forwardTargetServiceType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>
    /// if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    public static void TryAddKeyedTransientForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey)
        where TService : class
        where TForwardTargetService : class, TService
    {
        services.TryAddKeyedTransientForward(typeof(TService), serviceKey, typeof(TForwardTargetService));
    }
}
