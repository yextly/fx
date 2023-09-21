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
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class UseRoleProviderAttribute : Attribute
    {
        /// <summary>
        /// Specifies the provider to use.
        /// </summary>
        public Type ProviderType { get; }

        /// <summary>
        /// Creates a new <see cref="UseRoleProviderAttribute"/> instance.
        /// </summary>
        /// <param name="providerType">The type of the provider to use.</param>
        public UseRoleProviderAttribute(Type providerType)
        {
            ProviderType = providerType ?? throw new ArgumentNullException(nameof(providerType));
        }
    }
}
