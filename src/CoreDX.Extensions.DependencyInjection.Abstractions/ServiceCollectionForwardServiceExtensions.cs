using CoreDX.Extensions.DependencyInjection;
using CoreDX.Extensions.DependencyInjection.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add service forward.
/// </summary>
public static partial class ServiceCollectionForwardServiceExtensions
{
    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddForward(
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
            services.Add(new TypedImplementationFactoryServiceDescriptor(serviceType, new OpenGenericImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }
        else
        {
            services.Add(new ServiceDescriptor(serviceType, new ImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddForward(typeof(TService), typeof(TForwardTargetService), serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedForward(
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
            services.Add(new TypedImplementationFactoryServiceDescriptor(serviceType, serviceKey, new KeyedOpenGenericImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }
        else
        {
            services.Add(new ServiceDescriptor(serviceType, serviceKey, new KeyedImplementationFactoryServiceTypeHolder(forwardTargetServiceType).Factory!, serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddKeyedForward(typeof(TService), serviceKey, typeof(TForwardTargetService), serviceLifetime);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSingletonForward(
        this IServiceCollection services,
        Type serviceType,
        Type forwardTargetServiceType)
    {
        return services.AddForward(serviceType, forwardTargetServiceType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSingletonForward<TService, TForwardTargetService>(this IServiceCollection services)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddSingletonForward(typeof(TService), typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedSingletonForward(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Type forwardTargetServiceType)
    {
        return services.AddKeyedForward(serviceType, serviceKey, forwardTargetServiceType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedSingletonForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddKeyedSingletonForward(typeof(TService), serviceKey, typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddScopedForward(
        this IServiceCollection services,
        Type serviceType,
        Type forwardTargetServiceType)
    {
        return services.AddForward(serviceType, forwardTargetServiceType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddScopedForward<TService, TForwardTargetService>(this IServiceCollection services)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddScopedForward(typeof(TService), typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedScopedForward(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Type forwardTargetServiceType)
    {
        return services.AddKeyedForward(serviceType, serviceKey, forwardTargetServiceType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedScopedForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddKeyedScopedForward(typeof(TService), serviceKey, typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTransientForward(
        this IServiceCollection services,
        Type serviceType,
        Type forwardTargetServiceType)
    {
        return services.AddForward(serviceType, forwardTargetServiceType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTransientForward<TService, TForwardTargetService>(this IServiceCollection services)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddTransientForward(typeof(TService), typeof(TForwardTargetService));
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <paramref name="serviceType"/> with a forward of the type
    /// specified in <paramref name="forwardTargetServiceType"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="forwardTargetServiceType">The forward type of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTransientForward(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Type forwardTargetServiceType)
    {
        return services.AddKeyedForward(serviceType, serviceKey, forwardTargetServiceType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a forward of the type
    /// specified in <typeparamref name="TForwardTargetService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TForwardTargetService">The type of the forward to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTransientForward<TService, TForwardTargetService>(
        this IServiceCollection services,
        object? serviceKey)
        where TService : class
        where TForwardTargetService : class, TService
    {
        return services.AddKeyedTransientForward(typeof(TService), serviceKey, typeof(TForwardTargetService));
    }
}
