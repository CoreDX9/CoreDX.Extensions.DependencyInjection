## About
Add service provider that supported service type is provided as argument in parameter of factory.

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

using ServiceProvider provider = services.BuildServiceProvider();

var a = provider.GetService<IA<int>>();
var b = provider.GetService<IB<int>>();


public interface IA<T>;

public interface IB<T> : IA<T>;

public class B<T> : IB<T>;
```

## Main Types
The main types provided by this library are:
* `CoreDX.Extensions.DependencyInjection.TypedImplementationFactoryServiceProvider`
* `CoreDX.Extensions.DependencyInjection.TypedImplementationFactoryServiceProviderFactory`
* `CoreDX.Extensions.DependencyInjection.ServiceCollectionForwardServiceExtensions`

# Related Packages
* `CoreDX.Extensions.DependencyInjection.Abstractions`
