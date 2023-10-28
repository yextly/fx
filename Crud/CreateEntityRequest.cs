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
    /// Represents a create resource request.
    /// </summary>
    public sealed class CreateEntityRequest
    {
        /// <summary>
        /// Contains the data to create.
        /// </summary>
        [JsonPropertyName("data")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This is a DTO")]
        public ExpandoObject? Data { get; set; }
    }
}
