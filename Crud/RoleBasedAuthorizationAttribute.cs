// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Allows to add RBAC support to a controller.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class RoleBasedAuthorizationAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="RoleBasedAuthorizationAttribute"/> attribute.
        /// </summary>
        /// <param name="readRoleName">The role required for reading.</param>
        /// <param name="updateRoleName">The role required for editing.</param>
        /// <param name="createRoleName">The role required for creating.</param>
        /// <param name="deleteRoleName">The role required for deleting.</param>
        /// <exception cref="ArgumentException"></exception>
        public RoleBasedAuthorizationAttribute(string readRoleName, string? updateRoleName = null, string? createRoleName = null, string? deleteRoleName = null)
        {
            if (string.IsNullOrWhiteSpace(readRoleName))
            {
                throw new ArgumentException($"'{nameof(readRoleName)}' cannot be null or whitespace.", nameof(readRoleName));
            }

            ReadRoleName = readRoleName;
            UpdateRoleName = updateRoleName ?? readRoleName;
            CreateRoleName = createRoleName ?? readRoleName;
            DeleteRoleName = deleteRoleName ?? readRoleName;
        }

        /// <summary>
        /// Gets the role required for creating.
        /// </summary>
        public string CreateRoleName { get; }

        /// <summary>
        /// Gets the role required for deleting.
        /// </summary>
        public string DeleteRoleName { get; }

        /// <summary>
        /// Gets the role required for editing.
        /// </summary>
        public string UpdateRoleName { get; }

        /// <summary>
        /// Gets the role required for reading.
        /// </summary>
        public string ReadRoleName { get; }
    }
}
