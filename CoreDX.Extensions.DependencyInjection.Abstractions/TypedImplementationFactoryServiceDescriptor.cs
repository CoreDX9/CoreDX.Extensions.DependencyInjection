using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CoreDX.Extensions.DependencyInjection.Abstractions;

/// <inheritdoc />
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class TypedImplementationFactoryServiceDescriptor : ServiceDescriptor
{
    private object? _typedImplementationFactory;

    public Func<IServiceProvider, Type, object>? TypedImplementationFactory
    {
        get
        {
            if (IsKeyedService)
            {
                throw new InvalidOperationException("This service descriptor is keyed. Your service provider may not support keyed services.");
            }
            return (Func<IServiceProvider, Type, object>?)_typedImplementationFactory;
        }
    }

    private object? _typedKeyedImplementationFactory;

    public Func<IServiceProvider, object?, Type, object>? TypedKeyedImplementationFactory
    {
        get
        {
            if (!IsKeyedService)
            {
                throw new InvalidOperationException("This service descriptor is not keyed.");
            }
            return (Func<IServiceProvider, object?, Type, object>?)_typedKeyedImplementationFactory;
        }
    }

    /// <summary>
    /// Don't use this!
    /// </summary>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        object instance)
        : base(serviceType, instance)
    {
        ThrowCtor();
    }

    /// <summary>
    /// Don't use this!
    /// </summary>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        ServiceLifetime lifetime)
        : base(serviceType, implementationType, lifetime)
    {
        ThrowCtor();
    }

    /// <summary>
    /// Don't use this!
    /// </summary>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        object? serviceKey,
        object instance)
        : base(serviceType, serviceKey, instance)
    {
        ThrowCtor();
    }

    /// <summary>
    /// Don't use this!
    /// </summary>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        Func<IServiceProvider, object> factory,
        ServiceLifetime lifetime)
        : base(serviceType, factory, lifetime)
    {
        ThrowCtor();
    }

    /// <summary>
    /// Don't use this!
    /// </summary>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        object? serviceKey,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        ServiceLifetime lifetime)
        : base(serviceType, serviceKey, implementationType, lifetime)
    {
        ThrowCtor();
    }

    /// <summary>
    /// Don't use this!
    /// </summary>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, object> factory,
        ServiceLifetime lifetime)
        : base(serviceType, serviceKey, factory, lifetime)
    {
        ThrowCtor();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/> with the specified factory.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="factory">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        Func<IServiceProvider, Type, object> factory,
        ServiceLifetime lifetime)
        : base(serviceType, ThrowFactory, lifetime)
    {
        CheckOpenGeneric(serviceType);
        _typedImplementationFactory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/> with the specified factory.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="factory">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <inheritdoc />
    public TypedImplementationFactoryServiceDescriptor(
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> factory,
        ServiceLifetime lifetime)
        : base(serviceType, serviceKey, ThrowKeyedFactory, lifetime)
    {
        CheckOpenGeneric(serviceType);
        _typedKeyedImplementationFactory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="factory">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor Singleton(
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationType)
    {
        return new(serviceType, implementationType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typ name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor Singleton<TService>(
        Func<IServiceProvider, Type, object> implementationType)
        where TService : class
    {
        return new(typeof(TService), implementationType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor KeyedSingleton(
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationType)
    {
        return new(serviceType, serviceKey, implementationType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor KeyedSingleton<TService>(
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationType)
        where TService : class
    {
        return new(typeof(TService), serviceKey, implementationType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor Scoped(
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationType)
    {
        return new(serviceType, implementationType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typ name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor Scoped<TService>(
        Func<IServiceProvider, Type, object> implementationType)
        where TService : class
    {
        return new(typeof(TService), implementationType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor KeyedScoped(
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationType)
    {
        return new(serviceType, serviceKey, implementationType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor KeyedScoped<TService>(
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationType)
        where TService : class
    {
        return new(typeof(TService), serviceKey, implementationType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor Transient(
        Type serviceType,
        Func<IServiceProvider, Type, object> implementationType)
    {
        return new(serviceType, implementationType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typ name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor Transient<TService>(
        Func<IServiceProvider, Type, object> implementationType)
        where TService : class
    {
        return new(typeof(TService), implementationType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor KeyedTransient(
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationType)
    {
        return new(serviceType, serviceKey, implementationType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Creates an instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>
    /// with the specified service in <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <param name="implementationType">A factory used for creating service instances. Requested service type is provided as argument in parameter of factory.</param>
    /// <returns>A new instance of <see cref="TypedImplementationFactoryServiceDescriptor"/>.</returns>
    public static TypedImplementationFactoryServiceDescriptor KeyedTransient<TService>(
        object? serviceKey,
        Func<IServiceProvider, object?, Type, object> implementationType)
        where TService : class
    {
        return new(typeof(TService), serviceKey, implementationType, ServiceLifetime.Transient);
    }

    private string DebuggerToString()
    {
        string text = $"Lifetime = {Lifetime}, ServiceType = \"{ServiceType.FullName}\"";
        if (IsKeyedService)
        {
            text += $", ServiceKey = \"{ServiceKey}\"";

            return text + $", TypedKeyedImplementationFactory = {TypedKeyedImplementationFactory!.Method}";
        }
        else
        {
            return text + $", TypedImplementationFactory = {TypedImplementationFactory!.Method}";
        }
    }

    private static void ThrowCtor()
    {
        throw new NotSupportedException($"{nameof(TypedImplementationFactoryServiceDescriptor)} only use for typed factory.");
    }

    private static object ThrowFactory(IServiceProvider serviceProvider)
    {
        throw new InvalidOperationException("Please use typed factory instead.");
    }

    private static object ThrowKeyedFactory(IServiceProvider serviceProvider, object? serviceKey)
    {
        throw new InvalidOperationException("Please use typed keyed factory instead.");
    }

    private static void CheckOpenGeneric(Type serviceType)
    {
        if (!serviceType.IsGenericTypeDefinition)
            throw new InvalidOperationException($"{nameof(TypedImplementationFactoryServiceDescriptor)} only used for generic type definition(open generic type).");
    }
}
