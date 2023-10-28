// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    // this is for swagger only
    /// <summary>
    /// <remarks>Paging considers applied filters.</remarks>
    /// </summary>
    public sealed class CrudCollectionResult
    {
        /// <summary>
        /// Gets the count in the collection.
        /// </summary>
        /// <remarks>The count reflects the applied filters.</remarks>
        [Required]
        [JsonPropertyName("count")]
        public int Count { get; init; }

        /// <summary>
        /// The returned data.
        /// </summary>
        [Required]
        [JsonPropertyName("data")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "This is a DTO")]
        public List<object> Data { get; init; } = new List<object>();
    }

    /// <summary>
    /// Represents a collection of data.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class CrudCollectionResult<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the count in the collection.
        /// </summary>
        /// <remarks>The count reflects the applied filters.</remarks>
        [Required]
        [JsonPropertyName("count")]
        public int Count { get; init; }

        /// <summary>
        /// The collection of data returned.
        /// </summary>
        [Required]
        [JsonPropertyName("data")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "This is a DTO")]
        public List<TEntity> Data { get; init; } = new List<TEntity>();
    }
}
