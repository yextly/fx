namespace Yextly.ServiceFabric.WellKnownIdentifiers
{
    /// <summary>
    /// Known Service Fabric roles.
    /// </summary>
    public static class KnownRoles
    {
        /// <summary>
        /// Read Crud Resources.
        /// </summary>
        public static readonly Guid CrudResourceOperator = new(KnownRolesAsString.CrudResourceOperator);

        /// <summary>
        /// Impersonate User Assigned Identity.
        /// </summary>
        public static readonly Guid IdentityImpersonateOperator = new(KnownRolesAsString.IdentityImpersonateOperator);

        /// <summary>
        /// Create, Read, Update, and Delete User Assigned Identity.
        /// </summary>
        /// <remarks>This role is compatible with Azure.</remarks>
        public static readonly Guid ManagedIdentityContributor = new(KnownRolesAsString.ManagedIdentityContributor);

        /// <summary>
        /// Read and Assign User Assigned Identity.
        /// </summary>
        /// <remarks>This role is compatible with Azure.</remarks>
        public static readonly Guid ManagedIdentityOperator = new(KnownRolesAsString.ManagedIdentityOperator);

        /// <summary>
        /// Read api and network routes.
        /// </summary>
        public static readonly Guid NetworkRouteOperator = new(KnownRolesAsString.NetworkRouteOperator);

        /// <summary>
        /// Indicates a role that is never given.
        /// </summary>
        /// <remarks>You can specify this role to bypass the formal validation checks on a role. This role <b>must never</b> be registered.</remarks>
        public static readonly Guid NoRole = new(KnownRolesAsString.NoRole);

        /// <summary>
        /// Allows for read, write and delete storage and entities.
        /// </summary>
        public static readonly Guid StorageContributor = new(KnownRolesAsString.StorageContributor);

        /// <summary>
        /// Allows for read access storage and entities.
        /// </summary>
        public static readonly Guid StorageReader = new(KnownRolesAsString.StorageReader);
    }
}
