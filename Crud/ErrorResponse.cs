// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Text.Json.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents an error reponse.
    /// </summary>
    public sealed class ErrorResponse
    {
        /// <summary>
        /// Contains the error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;
    }
}
