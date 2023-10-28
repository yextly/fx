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
    /// Contains sorting information for a single property (or column).
    /// </summary>
    public sealed class SortFieldInfo
    {
        /// <summary>
        /// The name of the column to sort.
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Contains how <see cref="Name"/> must be sorted.
        /// </summary>
        [JsonPropertyName("type")]
        public SortType SortType { get; set; }
    }
}
