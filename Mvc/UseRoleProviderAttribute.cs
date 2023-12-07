// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc
{
    /// <summary>
    /// Instructs the advertiser to dynamically obtain the roles by invoking the indicated provider.
    /// </summary>
    /// <remarks>
    /// Creates a new <see cref="UseRoleProviderAttribute"/> instance.
    /// </remarks>
    /// <param name="providerType">The type of the provider to use.</param>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class UseRoleProviderAttribute(Type providerType) : Attribute
    {
        /// <summary>
        /// Specifies the provider to use.
        /// </summary>
        public Type ProviderType { get; } = providerType ?? throw new ArgumentNullException(nameof(providerType));
    }
}
