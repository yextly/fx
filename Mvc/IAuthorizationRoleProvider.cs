// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Yextly.ServiceFabric.Mvc
{
    /// <summary>
    /// Allows the advertiser to dynamically obtain the roles.
    /// </summary>
    public interface IAuthorizationRoleProvider
    {
        /// <summary>
        /// Gets the role for the specific action.
        /// </summary>
        /// <param name="actionDescriptor">The action whose role is to be retrieved.</param>
        /// <returns>The role to use, or null when no role could be found.</returns>
        (bool AttributeFound, string? Role) GetRouteRole(ActionDescriptor actionDescriptor);
    }
}
