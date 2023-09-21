// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents a <see cref="RestResourceControllerBase{TInnerEntity, TOuterEntity}.GetList(GetListRequest)"/> request.
    /// </summary>
    public sealed class GetListRequest
    {
        /// <summary>
        /// Specifies the sorting options.
        /// </summary>
        [JsonPropertyName("sort")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is a DTO")]
        public SortFieldInfo[]? Sorting { get; set; }

        /// <summary>
        /// Specifies the pagination options.
        /// </summary>
        [JsonPropertyName("range")]
        public RangeInfo? Range { get; set; }

        /// <summary>
        /// Specifies the filtering options.
        /// </summary>
        [JsonPropertyName("filter")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is a DTO")]
        public FilterFieldInfo[]? Filter { get; set; }
    }
}
