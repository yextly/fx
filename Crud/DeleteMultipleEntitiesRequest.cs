// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents a multiple resource delete request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DeleteMultipleEntitiesRequest<T> where T : class
    {
        /// <summary>
        /// Contains the ids to delete.
        /// </summary>
        [JsonPropertyName("ids")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is a DTO")]
        public string[] Ids { get; set; } = default!;
    }
}
