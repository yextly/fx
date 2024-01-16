// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    internal sealed record Delays
    {
        public TimeSpan AsyncReplyDelay { get; init; }
        public TimeSpan SyncReplyDelay { get; init; }
    }
}
