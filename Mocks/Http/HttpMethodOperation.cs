// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Contains all the supported HTTP methods.
    /// </summary>
    public enum HttpMethodOperation
    {
        /// <summary>
        /// HTTP Get request.
        /// </summary>
        Get = 0,

        /// <summary>
        /// HTTP Post request.
        /// </summary>
        Post = 1,

        /// <summary>
        /// HTTP Put request.
        /// </summary>
        Put = 2,

        /// <summary>
        /// HTTP Delete request.
        /// </summary>
        Delete = 3,

        /// <summary>
        /// HTTP Patch request.
        /// </summary>
        Patch = 4,

        /// <summary>
        /// HTTP Connect request.
        /// </summary>
        Connect = 5,

        /// <summary>
        /// HTTP Head request.
        /// </summary>
        Head = 6,

        /// <summary>
        /// HTTP Options request.
        /// </summary>
        Options = 7,

        /// <summary>
        /// HTTP Trace request.
        /// </summary>
        Trace = 8,
    }
}
