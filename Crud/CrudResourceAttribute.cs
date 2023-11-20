// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Indicates a controller is responsible for handling a CRUD resource.
    /// </summary>
    /// <remarks>
    /// Constructs a new <see cref="CrudResourceAttribute"/> instance.
    /// </remarks>
    /// <param name="resourceId">Specifies the resource id in <see cref="Guid"/> format.</param>
    /// <param name="description">Specifies the description of the resource.</param>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CrudResourceAttribute(string resourceId, string description) : Attribute
    {
        /// <summary>
        /// Gets the resource id as a <see cref="Guid"/>.
        /// </summary>
        public string ResourceId { get; } = resourceId;

        /// <summary>
        /// Gets the description of the resource.
        /// </summary>
        public string Description { get; } = description;
    }
}
