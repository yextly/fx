// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc
{
    /// <summary>
    /// Contains general policies that are enforced at gateway level.
    /// </summary>
    public enum KnownPolicies
    {
        /// <summary>
        /// Unrestricted access.
        /// </summary>
        Unrestricted = 0,

        /// <summary>
        /// The route is not available and is not routable.
        /// </summary>
        NoAccess = 1,

        /// <summary>
        /// Authentication token only validation is required to access the route.
        /// </summary>
        Authenticated = 2,

        /// <summary>
        /// Authentication and claim-based validation is required to access the route.
        /// </summary>
        Claim = 3,

        /// <summary>
        /// Authentication and role-based validation is required to access the route.
        /// </summary>
        Role = 4,
    }
}
