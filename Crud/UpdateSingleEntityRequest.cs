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
    /// Represents a request to edit a single entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class UpdateSingleEntityRequest<T> where T : class
    {
        /// <summary>
        /// The fields to update.
        /// </summary>
        [JsonPropertyName("data")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This is a DTO")]
        public ExpandoObject Data { get; set; } = default!;

        /// <summary>
        /// The identifier of the entity to update.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The original fields prior the update.
        /// </summary>
        [JsonPropertyName("previousData")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This is a DTO")]
        public ExpandoObject? PreviousData { get; set; }
    }
}
