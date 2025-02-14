## About
Add service descriptor that service type is provided as argument in parameter of factory.

## How to Use
``` csharp
ServiceCollection services = new ();

// Get service type from implementation factory.
services.AddScoped(typeof(IB<>, (provider, requestServiceType) =>
{
    var closedType = typeof(B<>).MakeGenericType(requestServiceType.GenericTypeArguments);
    return Activator.CreateInstance(closedType);

    // If service is another registered service type.
    // return provider.GetService(closedType);
});

// Forward an open generic service to another open generic service.
services.AddScopedForward(typeof(IA<>), typeof(IB<>));


public interface IA<T>;

public interface IB<T> : IA<T>;

public class B<T> : IB<T>;
```

## Main Types
The main types provided by this library are:
* `CoreDX.Extensions.DependencyInjection.Abstractions.TypedImplementationFactoryServiceDescriptor`

## Related Packages
* `Microsoft.Extensions.DependencyInjection.Abstractions`
* `CoreDX.Extensions.DependencyInjection`
