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
    /// Specifies how to page the data.
    /// </summary>
    /// <remarks>Paging considers applied filters.</remarks>
    public sealed class RangeInfo
    {
        /// <summary>
        /// Indicates the first (inclusive) record to retrieve.
        /// </summary>
        /// <remarks>Paging considers applied filters.</remarks>
        [Required]
        [JsonPropertyName("start")]
        public int Start { get; set; }

        /// <summary>
        /// Indicates the last (inclusive) record to retrieve.
        /// </summary>
        /// <remarks>Paging considers applied filters.</remarks>
        [Required]
        [JsonPropertyName("end")]
        public int End { get; set; }
    }
}
