using CoreDX.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add service using typed service implementation factory.
/// </summary>
public static partial class ServiceCollectionTypedFactoryServiceExtensions
{
    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
    {
        services.Add(new TypedImplementationFactoryServiceDescriptor(serviceType, implementationFactory, serviceLifetime));
        return services;
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactory<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
    {
        return services.AddTypedFactory(typeof(TService), implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddTypedFactory(typeof(TService), implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
    {
        services.Add(new TypedImplementationFactoryServiceDescriptor(serviceType, serviceKey, implementationFactory, serviceLifetime));
        return services;
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactory<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
    {
        return services.AddKeyedTypedFactory(typeof(TService), serviceKey, implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds a service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactory<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddKeyedTypedFactory(typeof(TService), serviceKey, implementationFactory, serviceLifetime);
    }

    /// <summary>
    /// Adds service as <see cref="ServiceLifetime.Singleton"/> a service of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSingletonTypedFactory(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory)
    {
        return services.AddTypedFactory(serviceType, implementationFactory, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds service as <see cref="ServiceLifetime.Singleton"/> a service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactorySingleton<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory)
        where TService : class
    {
        return services.AddSingletonTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactorySingleton<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddSingletonTypedFactory(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds service as <see cref="ServiceLifetime.Singleton"/> a service of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactorySingleton(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
    {
        return services.AddKeyedTypedFactory(serviceType, serviceKey, implementationFactory, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactorySingleton<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
        where TService : class
    {
        return services.AddKeyedTypedFactorySingleton(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Singleton"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactorySingleton<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddKeyedTypedFactorySingleton(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds service as <see cref="ServiceLifetime.Scoped"/> a service of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactoryScoped(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory)
    {
        return services.AddTypedFactory(serviceType, implementationFactory, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds service as <see cref="ServiceLifetime.Scoped"/> a service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactoryScoped<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory)
        where TService : class
    {
        return services.AddTypedFactoryScoped(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactoryScoped<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddTypedFactoryScoped(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds service as <see cref="ServiceLifetime.Scoped"/> a service of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactoryScoped(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
    {
        return services.AddKeyedTypedFactory(serviceType, serviceKey, implementationFactory, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactoryScoped<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
        where TService : class
    {
        return services.AddKeyedTypedFactoryScoped(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Scoped"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactoryScoped<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddKeyedTypedFactoryScoped(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactoryTransient(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationFactory)
    {
        return services.AddTypedFactory(serviceType, implementationFactory, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactoryTransient<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, object> implementationFactory)
        where TService : class
    {
        return services.AddTypedFactoryTransient(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddTypedFactoryTransient<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddTypedFactoryTransient(typeof(TService), implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <paramref name="serviceType"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactoryTransient(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
    {
        return services.AddKeyedTypedFactory(serviceType, serviceKey, implementationFactory, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactoryTransient<TService>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationFactory)
        where TService : class
    {
        return services.AddKeyedTypedFactoryTransient(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a service as <see cref="ServiceLifetime.Transient"/> service of the type specified in <typeparamref name="TService"/> with a factory
    /// specified in <paramref name="implementationFactory"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddKeyedTypedFactoryTransient<TService, TImplementation>(
        this IServiceCollection services,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddKeyedTypedFactoryTransient(typeof(TService), serviceKey, implementationFactory);
    }

    /// <summary>
    /// Adds a <see cref="TypedImplementationFactoryServiceDescriptor"/> if an existing descriptor with the same
    /// <see cref="ServiceDescriptor.ServiceType"/> and an implementation that does not already exist
    /// in <paramref name="services"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="descriptor">The <see cref="TypedImplementationFactoryServiceDescriptor"/>.</param>
    /// <remarks>
    /// Use <see cref="TryAddEnumerable(IServiceCollection, TypedImplementationFactoryServiceDescriptor)"/> when registering a service implementation of a
    /// service type that
    /// supports multiple registrations of the same service type. Using
    /// <see cref="ServiceCollectionDescriptorExtensions.Add(IServiceCollection, ServiceDescriptor)"/> is not idempotent and can add
    /// duplicate
    /// <see cref="ServiceDescriptor"/> instances if called twice. Using
    /// <see cref="TryAddEnumerable(IServiceCollection, TypedImplementationFactoryServiceDescriptor)"/> will prevent registration
    /// of multiple implementation types.
    /// </remarks>
    public static void TryAddEnumerable(
        this IServiceCollection services,
        TypedImplementationFactoryServiceDescriptor descriptor)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(descriptor);
#else
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }
#endif

        Type? implementationType = descriptor.GetImplementationType();

        if (implementationType == typeof(object) ||
            implementationType == descriptor.ServiceType)
        {
            throw new ArgumentException(
                $"Implementation type cannot be '{implementationType.GetType()}' because it is indistinguishable from other services registered for '{descriptor.ServiceType}'.",
                nameof(descriptor));
        }

        int count = services.Count;
        for (int i = 0; i < count; i++)
        {
            TypedImplementationFactoryServiceDescriptor? service = services[i] as TypedImplementationFactoryServiceDescriptor;
            if (service?.ServiceType == descriptor.ServiceType &&
                service.GetImplementationType() == implementationType &&
                object.Equals(service.ServiceKey, descriptor.ServiceKey))
            {
                // Already added
                return;
            }
        }

        services.Add(descriptor);
    }

    /// <summary>
    /// Adds the specified <see cref="TypedImplementationFactoryServiceDescriptor"/>s if an existing descriptor with the same
    /// <see cref="ServiceDescriptor.ServiceType"/> and an implementation that does not already exist
    /// in <paramref name="services"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="descriptors">The <see cref="TypedImplementationFactoryServiceDescriptor"/>s.</param>
    /// <remarks>
    /// Use <see cref="TryAddEnumerable(IServiceCollection, TypedImplementationFactoryServiceDescriptor)"/> when registering a service
    /// implementation of a service type that
    /// supports multiple registrations of the same service type. Using
    /// <see cref="ServiceCollectionDescriptorExtensions.Add(IServiceCollection, ServiceDescriptor)"/> is not idempotent and can add
    /// duplicate
    /// <see cref="ServiceDescriptor"/> instances if called twice. Using
    /// <see cref="TryAddEnumerable(IServiceCollection, TypedImplementationFactoryServiceDescriptor)"/> will prevent registration
    /// of multiple implementation types.
    /// </remarks>
    public static void TryAddEnumerable(
        this IServiceCollection services,
        IEnumerable<TypedImplementationFactoryServiceDescriptor> descriptors)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(descriptors);
#else
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (descriptors is null)
        {
            throw new ArgumentNullException(nameof(descriptors));
        }
#endif

        foreach (TypedImplementationFactoryServiceDescriptor? d in descriptors)
        {
            services.TryAddEnumerable(d);
        }
    }
}
