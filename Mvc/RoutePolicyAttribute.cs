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
    /// <remarks>
    /// Creates a new <see cref="RoutePolicyAttribute"/> instance.
    /// </remarks>
    /// <param name="knownPolicy">The policy to apply.</param>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class RoutePolicyAttribute(KnownPolicies knownPolicy) : Attribute
    {
        /// <summary>
        /// The policy to apply to the route.
        /// </summary>
        public KnownPolicies KnownPolicy { get; } = knownPolicy;
    }
}
