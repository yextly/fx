// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc
{
    /// <summary>
    /// Specifies a routing policy for a route.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class RoutePolicyAttribute : Attribute
    {
        /// <summary>
        /// The policy to apply to the route.
        /// </summary>
        public KnownPolicies KnownPolicy { get; }

        /// <summary>
        /// Creates a new <see cref="RoutePolicyAttribute"/> instance.
        /// </summary>
        /// <param name="knownPolicy">The policy to apply.</param>
        public RoutePolicyAttribute(KnownPolicies knownPolicy)
        {
            KnownPolicy = knownPolicy;
        }
    }
}
