// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents a single resource delete request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DeleteEntityRequest<T> where T : class
    {
        /// <summary>
        /// Contains the id to delete.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        /// <summary>
        /// Contains the previous data.
        /// </summary>
        [JsonPropertyName("previousData")]
        public T? PreviousData { get; set; }
    }
}
