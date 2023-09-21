// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Provides role support for <see cref="CrudResourceControllerBase{TInnerEntity, TOuterEntity}"/>.
    /// <para>
    ///     This is an internal API that supports the Service Factory infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Service Factory release.
    /// </para>
    /// </summary>
    public sealed class CrudResourceRoleProvider : IAuthorizationRoleProvider
    {
        /// <inheritdoc/>
        public (bool AttributeFound, string? Role) GetRouteRole(ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is null)
            {
                throw new ArgumentNullException(nameof(actionDescriptor));
            }

            var rbac = (RoleBasedAuthorizationAttribute?)actionDescriptor.EndpointMetadata.FirstOrDefault(x => x is RoleBasedAuthorizationAttribute);
            var rop = (CrudOperationTypeAttribute?)actionDescriptor.EndpointMetadata.FirstOrDefault(x => x is CrudOperationTypeAttribute);

            if (rbac == null || rop == null)
                return (false, null);

            return rop.OperationType switch
            {
                OperationType.Read => (true, rbac.ReadRoleName),
                OperationType.Update => (true, rbac.UpdateRoleName),
                OperationType.Delete => (true, rbac.DeleteRoleName),
                OperationType.Create => (true, rbac.CreateRoleName),
                _ => throw new NotSupportedException($"Unknown type {(int)rop.OperationType}"),
            };
        }
    }
}
