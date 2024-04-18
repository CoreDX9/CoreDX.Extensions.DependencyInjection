using CoreDX.Extensions.DependencyInjection.Abstractions;

namespace Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to try add service using typed service implementation factory.
/// </summary>
public static partial class ServiceCollectionTypedFactoryServiceExtensions
{
    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/>
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationFactory);
#else
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (implementationFactory is null)
        {
            throw new ArgumentNullException(nameof(implementationFactory));
        }
#endif

        services.TryAdd(new TypedImplementationFactoryServiceDescriptor(serviceType, implementationFactory, serviceLifetime));
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/>
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddTypedFactory<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
    {
        services.TryAddTypedFactory(typeof(TService), implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/>
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddTypedFactory(typeof(TService), implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds the specified service using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationFactory);
#else
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (implementationFactory is null)
        {
            throw new ArgumentNullException(nameof(implementationFactory));
        }
#endif

        services.TryAdd(new TypedImplementationFactoryServiceDescriptor(serviceType, serviceKey, implementationFactory, serviceLifetime));
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/>
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedTypedFactory<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
    {
        services.TryAddKeyedTypedFactory(typeof(TService), serviceKey, implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/>
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddKeyedTypedFactory(typeof(TService), serviceKey, implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> as <see cref="ServiceLifetime.Singleton"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddSingletonTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory)
    {
        services.TryAddTypedFactory(serviceType, implementationFactory, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Singleton"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddSingletonTypedFactory<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory)
        where TService : class
    {
        services.TryAddSingletonTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Singleton"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddSingletonTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddSingletonTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> as <see cref="ServiceLifetime.Singleton"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedSingletonTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
    {
        services.TryAddKeyedTypedFactory(serviceType, serviceKey, implementationFactory, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Singleton"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedSingletonTypedFactory<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
        where TService : class
    {
        services.TryAddKeyedSingletonTypedFactory(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Singleton"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedSingletonTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddKeyedSingletonTypedFactory(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> as <see cref="ServiceLifetime.Scoped"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddScopedTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory)
    {
        services.TryAddTypedFactory(serviceType, implementationFactory, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Scoped"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddScopedTypedFactory<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory)
        where TService : class
    {
        services.TryAddScopedTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Scoped"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddScopedTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddScopedTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> as <see cref="ServiceLifetime.Scoped"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedScopedTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
    {
        services.TryAddKeyedTypedFactory(serviceType, serviceKey, implementationFactory, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Scoped"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedScopedTypedFactory<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
        where TService : class
    {
        services.TryAddKeyedScopedTypedFactory(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Scoped"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedScopedTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddKeyedScopedTypedFactory(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> as <see cref="ServiceLifetime.Transient"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddTransientTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory)
    {
        services.TryAddTypedFactory(serviceType, implementationFactory, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Transient"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddTransientTypedFactory<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory)
        where TService : class
    {
        services.TryAddTransientTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Transient"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddTransientTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddTransientTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> as <see cref="ServiceLifetime.Transient"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedTransientTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
    {
        services.TryAddKeyedTypedFactory(serviceType, serviceKey, implementationFactory, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Transient"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedTransientTypedFactory<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
        where TService : class
    {
        services.TryAddKeyedTransientTypedFactory(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> as <see cref="ServiceLifetime.Transient"/> service
    /// using the factory specified in <paramref name="implementationFactory"/> to the
    /// specified <see cref="IServiceCollection"/> if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static void TryAddKeyedTransientTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddKeyedTransientTypedFactory(typeof(TService), serviceKey, implementationFactory);
    }
}
