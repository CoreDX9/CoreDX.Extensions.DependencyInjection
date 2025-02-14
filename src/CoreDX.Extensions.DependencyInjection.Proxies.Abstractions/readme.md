## About
Add service descriptor extensions for register dynamic proxy.

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

// Add explicit proxy to services.
services.AddScopedExplicitProxy(typeof(IA<>), typeof(MyInterceptor));

// Add implicit proxy to services.
services.AddScopedImplicitProxy(typeof(IB<>), typeof(MyInterceptor));

// Add keyed implicit proxy to services.
services.AddScopedImplicitProxy(typeof(IB<>), "key", typeof(MyInterceptor));

// Solidify open generic service proxy register at last.
services.SolidifyOpenGenericServiceProxyRegister();

using ServiceProvider provider = services.BuildServiceProvider();

// Get explicit proxy service using interface IProxyService<TService>.
var a = provider.GetService<IProxyService<IA<int>>>();

// Get implicit proxy service.
var b = provider.GetService<IB<int>>();

// Get original service using typed key or constant string.
var originalB1 = provider.GetKeyedService<IB<int>>(ImplicitProxyServiceOriginalServiceKey.StringDefault);
var originalB2 = provider.GetKeyedService<IB<int>>(ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix);

// Get keyed original service using typed key or constant string.
var originalB3 = provider.GetKeyedService<IB<int>>(ImplicitProxyServiceOriginalServiceKey.CreateStringOriginalServiceKey("key"));
var originalB4 = provider.GetKeyedService<IB<int>>($"{ImplicitProxyServiceOriginalServiceKey.DefaultStringPrefix}{"key"}");


public interface IA<T>;

public interface IB<T> : IA<T>;

public class B<T> : IB<T>;

public class MyInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine("MyInterceptorBefore");
        invocation.Proceed();
        Console.WriteLine("MyInterceptorAfter");
    }
}
```

## Main Types
The main types provided by this library are:
* `CoreDX.Extensions.DependencyInjection.Proxies.ImplicitProxyServiceOriginalServiceKey`
* `Microsoft.Extensions.DependencyInjection.CastleDynamicProxyDependencyInjectionExtensions`

## Related Packages
* `Castle.Core.AsyncInterceptor`
* `CoreDX.Extensions.DependencyInjection.Abstractions`
