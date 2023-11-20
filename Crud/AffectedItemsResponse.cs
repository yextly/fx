// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents a reponse that contains the affected entities.
    /// </summary>
    public sealed class AffectedItemsResponse
    {
        /// <summary>
        /// The identifiers of the affected entities.
        /// </summary>
        [JsonPropertyName("ids")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is a DTO")]
        public string[] Ids { get; set; } = [];
    }
}
