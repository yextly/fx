// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents a <see cref="CrudResourceControllerBase{TInnerEntity, TOuterEntity}.GetMultiple(GetMultipleEntitiesRequest)"/> request.
    /// </summary>
    public sealed class GetMultipleEntitiesRequest
    {
        /// <summary>
        /// Contains the list of identifiers to return.
        /// </summary>
        [Required]
        [JsonPropertyName("ids")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is a DTO")]
        public string[] Ids { get; set; } = Array.Empty<string>();
    }
}
