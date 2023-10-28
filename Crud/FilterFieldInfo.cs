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
    /// Contains filtering information for a single property (or column).
    /// </summary>
    public sealed class FilterFieldInfo
    {
        /// <summary>
        /// The name of the column or property to filter.
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// An expression used to filter. The expression is evaluated literally unless the data type supports like-based filters.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }
}
