// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents the type of the provider.
    /// </summary>
    public enum ProviderType
    {
        /// <summary>
        /// Self managed provider.
        /// </summary>
        SelfManaged = 0,

        /// <summary>
        /// Entity Framework.
        /// </summary>
        EntityFramework = 1,
    }
}
