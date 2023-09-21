// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents a request to batch update multiple entities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class UpdateMultipleEntitiesRequest<T> where T : class
    {
        /// <summary>
        /// Contains the list of identifiers of the entities to update.
        /// </summary>
        [JsonPropertyName("ids")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is a DTO")]
        public string[] Ids { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The fields to update.
        /// </summary>
        [JsonPropertyName("data")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This is a DTO")]
        public ExpandoObject Data { get; set; } = default!;
    }
}
