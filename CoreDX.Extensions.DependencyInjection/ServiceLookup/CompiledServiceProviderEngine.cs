// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;

namespace CoreDX.Extensions.DependencyInjection.ServiceLookup
{
    internal abstract class CompiledServiceProviderEngine : ServiceProviderEngine
    {
#if IL_EMIT
        public ILEmitResolverBuilder ResolverBuilder { get; }
#else
        public ExpressionResolverBuilder ResolverBuilder { get; }
#endif

#if NET7_0_OR_GREATER
        [RequiresDynamicCode("Creates DynamicMethods")]
#endif
        public CompiledServiceProviderEngine(TypedImplementationFactoryServiceProvider provider)
        {
            ResolverBuilder = new(provider);
        }

        public override Func<ServiceProviderEngineScope, object?> RealizeService(ServiceCallSite callSite) => ResolverBuilder.Build(callSite);
    }
}
