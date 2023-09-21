namespace Yextly.ServiceFabric.WellKnownIdentifiers
{
    /// <summary>
    /// Known Service Fabric roles.
    /// </summary>
    public static class KnownRolesAsString
    {
        /// <summary>
        /// Read Crud Resources.
        /// </summary>
        public const string CrudResourceOperator = "d0d95db2-cab0-4214-b963-51a694cae044";

        /// <summary>
        /// Impersonate User Assigned Identity.
        /// </summary>
        public const string IdentityImpersonateOperator = "754f752a-9117-48ec-9d70-feb6f34d69c5";

        /// <summary>
        /// Create, Read, Update, and Delete User Assigned Identity.
        /// </summary>
        /// <remarks>This role is compatible with Azure.</remarks>
        public const string ManagedIdentityContributor = "e40ec5ca-96e0-45a2-b4ff-59039f2c2b59";

        /// <summary>
        /// Read and Assign User Assigned Identity.
        /// </summary>
        /// <remarks>This role is compatible with Azure.</remarks>
        public const string ManagedIdentityOperator = "f1a07417-d97a-45cb-824c-7a7467783830";

        /// <summary>
        /// Read api and network routes.
        /// </summary>
        public const string NetworkRouteOperator = "3bbe63f3-81ab-4411-882a-5c23d860045c";

        /// <summary>
        /// Indicates a role that is never given.
        /// </summary>
        /// <remarks>You can specify this role to bypass the formal validation checks on a role. This role <b>must never</b> be registered.</remarks>
        public const string NoRole = "a8bb3e1c-88ee-48be-995f-6ec558615acd";

        /// <summary>
        /// Allows for read, write and delete storage and entities.
        /// </summary>
        public const string StorageContributor = "aeb6d4c8-8ef5-4a27-8dd7-e628d2b838dc";

        /// <summary>
        /// Allows for read access storage and entities.
        /// </summary>
        public const string StorageReader = "ac0d2834-ed7f-484e-9656-83a92dd76901";
    }
}
