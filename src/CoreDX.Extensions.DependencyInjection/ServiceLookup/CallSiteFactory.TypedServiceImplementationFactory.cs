using CoreDX.Extensions.DependencyInjection.Abstractions;

namespace CoreDX.Extensions.DependencyInjection.ServiceLookup;

internal sealed partial class CallSiteFactory
{
	/// <summary>
	/// 尝试创建可识别请求类型的工厂调用点
	/// </summary>
	/// <param name="lifetime"></param>
	/// <param name="descriptor"></param>
	/// <param name="serviceType">服务类型</param>
	/// <returns></returns>
	private static FactoryCallSite? TryCreateTypedFactoryCallSite(
		ResultCache lifetime,
		TypedImplementationFactoryServiceDescriptor? descriptor,
		Type serviceType)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(serviceType);
#else
        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }
#endif

        if (descriptor == null) { }
		else if (descriptor.IsKeyedService && descriptor.TypedKeyedImplementationFactory != null)
		{
			return new FactoryCallSite(lifetime, descriptor.ServiceType, descriptor.ServiceKey!, new TypedKeyedServiceImplementationFactoryHolder(descriptor.TypedKeyedImplementationFactory!, serviceType).Factory);
		}
		else if (!descriptor.IsKeyedService && descriptor.TypedImplementationFactory != null)
        {
			return new FactoryCallSite(lifetime, descriptor.ServiceType, new TypedServiceImplementationFactoryHolder(descriptor.TypedImplementationFactory!, serviceType).Factory);
		}

        return null;
	}
}
