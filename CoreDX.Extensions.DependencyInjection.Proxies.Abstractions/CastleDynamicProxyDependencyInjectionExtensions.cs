using Castle.DynamicProxy;
using CoreDX.Extensions.DependencyInjection.Abstractions;
using CoreDX.Extensions.DependencyInjection.Proxies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

public static class CastleDynamicProxyDependencyInjectionExtensions
{
    /// <summary>
    /// Adds a explicit proxy for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        CheckInterface(serviceType);
        CheckInterceptor(interceptorTypes);

        if (serviceType.IsGenericTypeDefinition)
        {
            services.TryAddSingleton<IStartupOpenGenericServiceProxyRegister>(new StartupOpenGenericServiceProxyRegister());

            var startupOpenGenericServiceProxyRegister = services
                .LastOrDefault(service => service.ServiceType == typeof(IStartupOpenGenericServiceProxyRegister))
                ?.ImplementationInstance as IStartupOpenGenericServiceProxyRegister
                ?? throw new InvalidOperationException($"Can not found service of type {nameof(IStartupOpenGenericServiceProxyRegister)}");

            startupOpenGenericServiceProxyRegister?.Add(serviceType);

            services.TryAdd(
                new TypedImplementationFactoryServiceDescriptor(
                    typeof(IProxyService<>),
                    (provider, requestedServiceType) =>
                    {
                        var proxyServiceType = requestedServiceType.GenericTypeArguments[0];

                        var registered = CheckOpenGenericServiceProxyRegister(provider, proxyServiceType.GetGenericTypeDefinition());
                        if (!registered) return null!;

                        var proxy = CreateProxyObject(provider, proxyServiceType, interceptorTypes);

                        return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(proxy.GetType()), proxy)!;
                    },
                    serviceLifetime)
                );
        }
        else
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(IProxyService<>).MakeGenericType(serviceType),
                    provider =>
                    {
                        var proxy = CreateProxyObject(provider, serviceType, interceptorTypes);
                        return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(proxy.GetType()), proxy)!;
                    },
                    serviceLifetime)
                );
        }

        services.TryAddInterceptors(serviceLifetime, interceptorTypes);

        return services;
    }

    /// <summary>
    /// Adds a explicit proxy for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddExplicitProxy<TService>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddExplicitProxy(typeof(TService), serviceLifetime, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        CheckInterface(serviceType);
        CheckInterceptor(interceptorTypes);

        var originalServiceDescriptor = services.LastOrDefault(service => service.IsKeyedService is false && service.ServiceType == serviceType && service.Lifetime == serviceLifetime)
            ?? throw new ArgumentException($"Not found registered \"{Enum.GetName(serviceLifetime)}\" service of type {serviceType.Name}.", nameof(serviceType));

        var serviceDescriptorIndex = services.IndexOf(originalServiceDescriptor);
        if (originalServiceDescriptor is TypedImplementationFactoryServiceDescriptor typedServiceDescriptor)
        {
            services.Insert(
                serviceDescriptorIndex,
                new TypedImplementationFactoryServiceDescriptor(
                    typedServiceDescriptor.ServiceType,
                    ImplicitProxyServiceOriginalServiceKey.StringDefault,
                    (serviceProvider, serviceKey, requestedServiceType) =>
                    {
                        return typedServiceDescriptor.TypedImplementationFactory!(serviceProvider, requestedServiceType);
                    },
                    originalServiceDescriptor.Lifetime)
                );
        }
        else if (originalServiceDescriptor.ImplementationInstance is not null)
        {
            services.Insert(
                serviceDescriptorIndex,
                new ServiceDescriptor(
                    originalServiceDescriptor.ServiceType,
                    ImplicitProxyServiceOriginalServiceKey.StringDefault,
                    originalServiceDescriptor.ImplementationInstance)
                );
        }
        else if (originalServiceDescriptor.ImplementationType is not null)
        {
            services.Insert(
                serviceDescriptorIndex,
                new ServiceDescriptor(
                    originalServiceDescriptor.ServiceType,
                    ImplicitProxyServiceOriginalServiceKey.StringDefault,
                    originalServiceDescriptor.ImplementationType,
                    originalServiceDescriptor.Lifetime)
                );
        }
        else if (originalServiceDescriptor.ImplementationFactory is not null)
        {
            services.Insert(
                serviceDescriptorIndex,
                new ServiceDescriptor(
                    originalServiceDescriptor.ServiceType,
                    ImplicitProxyServiceOriginalServiceKey.StringDefault,
                    (serviceProvider, serviceKey) =>
                    {
                        return originalServiceDescriptor.ImplementationFactory(serviceProvider);
                    },
                    originalServiceDescriptor.Lifetime)
                );
        }
        else throw new Exception("Add proxy service fail.");

        if (serviceType.IsGenericTypeDefinition)
        {
            services.Add(new TypedImplementationFactoryServiceDescriptor(
                serviceType,
                (provider, requestedServiceType) =>
                {
                    var proxy = CreateKeyedProxyObject(provider, requestedServiceType, ImplicitProxyServiceOriginalServiceKey.StringDefault, interceptorTypes);

                    return proxy;
                },
                serviceLifetime));
        }
        else
        {
            services.Add(new ServiceDescriptor(
                serviceType,
                provider =>
                {
                    var proxy = CreateKeyedProxyObject(provider, serviceType, ImplicitProxyServiceOriginalServiceKey.StringDefault, interceptorTypes);
                    return proxy;
                },
                serviceLifetime));
        }

        services.TryAddKeyedInterceptors(ImplicitProxyServiceOriginalServiceKey.StringDefault, serviceLifetime, interceptorTypes);

        services.Remove(originalServiceDescriptor);

        return services;
    }

    /// <summary>
    /// Adds a implicit proxy for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddImplicitProxy<TService>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddImplicitProxy(typeof(TService), serviceLifetime, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        CheckInterface(serviceType);
        CheckInterceptor(interceptorTypes);

        if (serviceType.IsGenericTypeDefinition)
        {
            services.TryAddKeyedSingleton<IStartupOpenGenericServiceProxyRegister>(serviceKey, new StartupOpenGenericServiceProxyRegister());

            var startupOpenGenericServiceProxyRegister = services
                .LastOrDefault(service => service.IsKeyedService && service.ServiceKey == serviceKey && service.ServiceType == typeof(IStartupOpenGenericServiceProxyRegister))
                ?.KeyedImplementationInstance as IStartupOpenGenericServiceProxyRegister
                ?? throw new InvalidOperationException($"Can not found keyed(key value: {serviceKey}) service of type {nameof(IStartupOpenGenericServiceProxyRegister)}");

            startupOpenGenericServiceProxyRegister?.Add(serviceType);

            services.TryAdd(new TypedImplementationFactoryServiceDescriptor(
                typeof(IProxyService<>),
                serviceKey,
                (provider, serviceKey, requestedServiceType) =>
                {
                    var proxyServiceType = requestedServiceType.GenericTypeArguments[0];

                    var registered = CheckKeyedOpenGenericServiceProxyRegister(provider, serviceKey, proxyServiceType.GetGenericTypeDefinition());
                    if (!registered) return null!;

                    var proxy = CreateKeyedProxyObject(provider, proxyServiceType, serviceKey, interceptorTypes);

                    return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(proxy.GetType()), proxy)!;
                },
                serviceLifetime));
        }
        else
        {
            services.Add(new ServiceDescriptor(
                typeof(IProxyService<>).MakeGenericType(serviceType),
                serviceKey,
                (provider, serviceKey) =>
                {
                    var proxy = CreateKeyedProxyObject(provider, serviceType, serviceKey, interceptorTypes);
                    return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(proxy.GetType()), proxy)!;
                },
                serviceLifetime));
        }

        services.TryAddKeyedInterceptors(serviceKey, serviceLifetime, interceptorTypes);

        return services;
    }

    /// <summary>
    /// Adds a explicit proxy for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedExplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedExplicitProxy(typeof(TService), serviceKey, serviceLifetime, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <paramref name="serviceType"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        CheckInterface(serviceType);
        CheckInterceptor(interceptorTypes);

        var originalServiceDescriptor = services.LastOrDefault(service => service.IsKeyedService && service.ServiceKey == serviceKey && service.ServiceType == serviceType && service.Lifetime == serviceLifetime)
            ?? throw new ArgumentException($"Not found registered keyed(key value: {serviceKey}) \"{Enum.GetName(serviceLifetime)}\" service of type {serviceType.Name}.", nameof(serviceType));

        var newServiceKey = CreateOriginalServiceKey(serviceKey);
        var serviceDescriptorIndex = services.IndexOf(originalServiceDescriptor);
        if (originalServiceDescriptor is TypedImplementationFactoryServiceDescriptor typedServiceDescriptor)
        {
            services.Insert(
                serviceDescriptorIndex,
                new TypedImplementationFactoryServiceDescriptor(
                    typedServiceDescriptor.ServiceType,
                    newServiceKey,
                    (serviceProvider, serviceKey, requestedServiceType) =>
                    {
                        Debug.Assert(serviceKey is ImplicitProxyServiceOriginalServiceKey, $"Implicit proxy not use {nameof(ImplicitProxyServiceOriginalServiceKey)}");

                        return typedServiceDescriptor.TypedKeyedImplementationFactory!(
                            serviceProvider,
                            (serviceKey as ImplicitProxyServiceOriginalServiceKey)?.OriginalServiceKey ?? serviceKey,
                            requestedServiceType);
                    },
                    originalServiceDescriptor.Lifetime)
                );
        }
        else if (originalServiceDescriptor.KeyedImplementationInstance is not null)
        {
            services.Insert(
                serviceDescriptorIndex,
                new ServiceDescriptor(
                    originalServiceDescriptor.ServiceType,
                    newServiceKey,
                    originalServiceDescriptor.KeyedImplementationInstance)
                );
        }
        else if (originalServiceDescriptor.KeyedImplementationType is not null)
        {
            services.Insert(
                serviceDescriptorIndex,
                new ServiceDescriptor(
                    originalServiceDescriptor.ServiceType,
                    newServiceKey,
                    originalServiceDescriptor.KeyedImplementationType,
                    originalServiceDescriptor.Lifetime)
                );
        }
        else if (originalServiceDescriptor.KeyedImplementationFactory is not null)
        {
            services.Insert(
                serviceDescriptorIndex,
                new ServiceDescriptor(
                    originalServiceDescriptor.ServiceType,
                    newServiceKey,
                    (serviceProvider, serviceKey) =>
                    {
                        return originalServiceDescriptor.KeyedImplementationFactory(
                            serviceProvider,
                            serviceKey);
                    },
                    originalServiceDescriptor.Lifetime)
                );
        }
        else throw new Exception("Add proxy service fail.");

        if (serviceType.IsGenericTypeDefinition)
        {
            services.Add(new TypedImplementationFactoryServiceDescriptor(
                serviceType,
                serviceKey,
                (provider, serviceKey, requestedServiceType) =>
                {
                    var newLocalServiceKey = CreateOriginalServiceKey(serviceKey);
                    var proxy = CreateKeyedProxyObject(provider, requestedServiceType, newLocalServiceKey, interceptorTypes);

                    return proxy;
                },
                serviceLifetime));
        }
        else
        {
            services.Add(new ServiceDescriptor(
                serviceType,
                serviceKey,
                (provider, serviceKey) =>
                {
                    var newLocalServiceKey = CreateOriginalServiceKey(serviceKey);
                    var proxy = CreateKeyedProxyObject(provider, serviceType, newLocalServiceKey, interceptorTypes);

                    return proxy;
                },
                serviceLifetime));
        }

        services.TryAddKeyedInterceptors(newServiceKey, serviceLifetime, interceptorTypes);

        services.Remove(originalServiceDescriptor);

        return services;
    }

    /// <summary>
    /// Adds a implicit proxy for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/> of <typeparamref name="TService"/> and <paramref name="interceptorTypes"/>.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedImplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        ServiceLifetime serviceLifetime,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedImplicitProxy(typeof(TService), serviceKey, serviceLifetime, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddSingletonExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        params Type[] interceptorTypes)
    {
        return services.AddExplicitProxy(serviceType, ServiceLifetime.Singleton, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddSingletonExplicitProxy<TService>(
        this IServiceCollection services,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddSingletonExplicitProxy(typeof(TService), interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddSingletonImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        params Type[] interceptorTypes)
    {
        return services.AddImplicitProxy(serviceType, ServiceLifetime.Singleton, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddSingletonImplicitProxy<TService>(
        this IServiceCollection services,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddSingletonImplicitProxy(typeof(TService), interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedSingletonExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        params Type[] interceptorTypes)
    {
        return services.AddKeyedExplicitProxy(serviceType, serviceKey, ServiceLifetime.Singleton, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedSingletonExplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedSingletonExplicitProxy(typeof(TService), serviceKey, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedSingletonImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        params Type[] interceptorTypes)
    {
        return services.AddKeyedImplicitProxy(serviceType, serviceKey, ServiceLifetime.Singleton, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Singleton"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedSingletonImplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedSingletonImplicitProxy(typeof(TService), serviceKey, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddScopedExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        params Type[] interceptorTypes)
    {
        return services.AddExplicitProxy(serviceType, ServiceLifetime.Scoped, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddScopedExplicitProxy<TService>(
        this IServiceCollection services,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddScopedExplicitProxy(typeof(TService), interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddScopedImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        params Type[] interceptorTypes)
    {
        return services.AddImplicitProxy(serviceType, ServiceLifetime.Scoped, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddScopedImplicitProxy<TService>(
        this IServiceCollection services,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddScopedImplicitProxy(typeof(TService), interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedScopedExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        params Type[] interceptorTypes)
    {
        return services.AddKeyedExplicitProxy(serviceType, serviceKey, ServiceLifetime.Scoped, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedScopedExplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedScopedExplicitProxy(typeof(TService), serviceKey, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedScopedImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        params Type[] interceptorTypes)
    {
        return services.AddKeyedImplicitProxy(serviceType, serviceKey, ServiceLifetime.Scoped, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Scoped"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedScopedImplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedScopedImplicitProxy(typeof(TService), serviceKey, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddTransientExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        params Type[] interceptorTypes)
    {
        return services.AddExplicitProxy(serviceType, ServiceLifetime.Transient, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddTransientExplicitProxy<TService>(
        this IServiceCollection services,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddTransientExplicitProxy(typeof(TService), interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddTransientImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        params Type[] interceptorTypes)
    {
        return services.AddImplicitProxy(serviceType, ServiceLifetime.Transient, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.StringDefault"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> (eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddTransientImplicitProxy<TService>(
        this IServiceCollection services,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddTransientImplicitProxy(typeof(TService), interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedTransientExplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        params Type[] interceptorTypes)
    {
        return services.AddKeyedExplicitProxy(serviceType, serviceKey, ServiceLifetime.Transient, interceptorTypes);
    }

    /// <summary>
    /// Adds a explicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use <see cref="IProxyService{TService}"/> to get proxy service.</remarks>
    public static IServiceCollection AddKeyedTransientExplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedTransientExplicitProxy(typeof(TService), serviceKey, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <paramref name="serviceType"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceType">The type of the service to add proxy.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedTransientImplicitProxy(
        this IServiceCollection services,
        Type serviceType,
        object? serviceKey,
        params Type[] interceptorTypes)
    {
        return services.AddKeyedImplicitProxy(serviceType, serviceKey, ServiceLifetime.Transient, interceptorTypes);
    }

    /// <summary>
    /// Adds a implicit proxy as <see cref="ServiceLifetime.Transient"/> service for the type specified in <typeparamref name="TService"/> with interceptors
    /// specified in <paramref name="interceptorTypes"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add proxy.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service proxy to.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="interceptorTypes">The interceptor types of the service proxy.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// Use key <see cref="ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(object?)"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(string?)"/> if <paramref name="serviceKey"/> is <see cref="string"/>
    /// or <see cref="ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix"/> + <paramref name="serviceKey"/> if <paramref name="serviceKey"/>
    /// is <see cref="string"/>(eg. Constant value for <see cref="FromKeyedServicesAttribute"/>.) to get original service.
    /// </remarks>
    public static IServiceCollection AddKeyedTransientImplicitProxy<TService>(
        this IServiceCollection services,
        object? serviceKey,
        params Type[] interceptorTypes)
        where TService : class
    {
        return services.AddKeyedTransientImplicitProxy(typeof(TService), serviceKey, interceptorTypes);
    }

    /// <summary>
    /// Solidify open generic service proxy register for the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="IServiceCollection"/> to solidify register.</param>
    /// <remarks>Should call after last add proxy. If used for host, needn't call.</remarks>
    public static void SolidifyOpenGenericServiceProxyRegister(this IServiceCollection containerBuilder)
    {
        var openGenericServiceProxyRegisters = containerBuilder
            .Where(service => service.ServiceType == typeof(IStartupOpenGenericServiceProxyRegister))
            .ToList();

        var readOnlyOpenGenericServiceProxyRegisters = openGenericServiceProxyRegisters
            .Where(service => service.Lifetime == ServiceLifetime.Singleton)
            .Select(service =>
            {
                return service.IsKeyedService switch
                {
                    true => ServiceDescriptor.KeyedSingleton<IOpenGenericServiceProxyRegister>(service.ServiceKey, new OpenGenericServiceProxyRegister((service.KeyedImplementationInstance! as IStartupOpenGenericServiceProxyRegister)!)),
                    false => ServiceDescriptor.Singleton<IOpenGenericServiceProxyRegister>(new OpenGenericServiceProxyRegister((service.ImplementationInstance! as IStartupOpenGenericServiceProxyRegister)!)),
                };
            });

        foreach (var register in openGenericServiceProxyRegisters)
        {
            containerBuilder.Remove(register);
        }

        foreach (var readOnlyRegister in readOnlyOpenGenericServiceProxyRegisters)
        {
            containerBuilder.Add(readOnlyRegister);
        }
    }

    private static object CreateProxyObject(
        IServiceProvider provider,
        Type serviceType,
        Type[] interceptorTypes)
    {
        var target = provider.GetRequiredService(serviceType);
        var interceptors = interceptorTypes.Select(t => GetInterceptor(provider.GetRequiredService(t))).ToArray();
        var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();

        var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, target, interceptors);
        return proxy;
    }

    private static object CreateKeyedProxyObject(
        IServiceProvider provider,
        Type serviceType,
        object? serviceKey,
        Type[] interceptorTypes)
    {
        var target = provider.GetRequiredKeyedService(serviceType, serviceKey);
        var interceptors = interceptorTypes.Select(t => GetInterceptor(provider.GetRequiredKeyedService(t, serviceKey))).ToArray();
        var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();

        var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, target, interceptors);
        return proxy;
    }

    private static ImplicitProxyServiceOriginalServiceKey CreateOriginalServiceKey(object? serviceKey)
    {
        return serviceKey switch
        {
            string stringKey => ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey(stringKey),
            _ => ImplicitProxyServiceOriginalServiceKey.CreateOriginalServiceKey(serviceKey)
        };
    }

    private static void TryAddInterceptors(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        params Type[] interceptorTypes)
    {
        services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();

        foreach (var interceptorType in interceptorTypes)
        {
            services.TryAdd(new ServiceDescriptor(interceptorType, interceptorType, lifetime));
        }
    }

    private static void TryAddKeyedInterceptors(
        this IServiceCollection services,
        object? serviceKey,
        ServiceLifetime lifetime,
        params Type[] interceptorTypes)
    {
        services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();

        foreach (var interceptorType in interceptorTypes)
        {
            services.TryAdd(new ServiceDescriptor(interceptorType, serviceKey, interceptorType, lifetime));
        }
    }

    private static IInterceptor GetInterceptor(object interceptor)
    {
        return (interceptor as IInterceptor)
            ?? (interceptor as IAsyncInterceptor)?.ToInterceptor()
            ?? throw new InvalidCastException($"{nameof(interceptor)} is not {nameof(IInterceptor)} or {nameof(IAsyncInterceptor)}.");       
    }

    private static void CheckInterface(Type serviceType)
    {
        if (!serviceType.IsInterface)
            throw new InvalidOperationException($"Proxy need interface but {nameof(serviceType)} is not interface.");
    }

    private static void CheckInterceptor(params Type[] types)
    {
        foreach (var type in types)
        {
            if (!(type.IsAssignableTo(typeof(IInterceptor)) || type.IsAssignableTo(typeof(IAsyncInterceptor))))
                throw new ArgumentException($"Exist element in {nameof(types)} is not {nameof(IInterceptor)} or {nameof(IAsyncInterceptor)}.", $"{nameof(types)}");
        }
    }

    private static bool CheckOpenGenericServiceProxyRegister(IServiceProvider serviceProvider, Type serviceType)
    {
        var register = serviceProvider.GetService<IOpenGenericServiceProxyRegister>();
        CheckOpenGenericServiceProxyRegister(register);
        return register!.Contains(serviceType);
    }

    private static bool CheckKeyedOpenGenericServiceProxyRegister(IServiceProvider serviceProvider, object? serviceKey, Type serviceType)
    {
        var register = serviceProvider.GetKeyedService<IOpenGenericServiceProxyRegister>(serviceKey);
        CheckOpenGenericServiceProxyRegister(register);
        return register!.Contains(serviceType);
    }

    private static void CheckOpenGenericServiceProxyRegister(IOpenGenericServiceProxyRegister? register)
    {
        if (register is null) throw new InvalidOperationException($"Can not found required service of type {nameof(IOpenGenericServiceProxyRegister)}. Maybe you forgot to call method named {nameof(IServiceCollection)}.{nameof(SolidifyOpenGenericServiceProxyRegister)}().");
    }

    private sealed class StartupOpenGenericServiceProxyRegister : List<Type>, IStartupOpenGenericServiceProxyRegister;
}
