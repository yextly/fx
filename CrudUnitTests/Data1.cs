// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;

namespace CrudUnitTests
{
    public sealed record Data1
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public int Introduction { get; set; }

        public string Allegiances { get; set; } = default!;

        public Guid IdAsGuid { get; set; }
    }
}
