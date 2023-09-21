// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Contains the representation of a resource.
    /// </summary>
    public sealed record CrudResourceDescriptor
    {
        /// <summary>
        /// Constructs a new <see cref="CrudResourceDescriptor"/>.
        /// </summary>
        /// <param name="id">Specifies the unique identifier of the resource.</param>
        /// <param name="name">Specifies the name of the resource.</param>
        /// <param name="description">Specifies the user-friendly description of the resource.</param>
        public CrudResourceDescriptor(string id, string name, string description)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(description);

            Id = id;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Represents the unique resource identifier (usually a guid).
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Represents the user-friendly name of the resource.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Represents the user-friendly description of the resource.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Specifies the roles necessary to create the resource.
        /// </summary>
        public ImmutableArray<string> CreateRoles { get; init; }

        /// <summary>
        /// Specifies the roles necessary to read the resource.
        /// </summary>
        public ImmutableArray<string> ReadRoles { get; init; }

        /// <summary>
        /// Specifies the roles necessary to update the resource.
        /// </summary>
        public ImmutableArray<string> UpdateRoles { get; init; }

        /// <summary>
        /// Specifies the roles necessary to delete the resource.
        /// </summary>
        public ImmutableArray<string> DeleteRoles { get; init; }
    }
}
