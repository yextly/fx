// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;

namespace CommonUnitTests
{
    internal sealed class DisposableObject : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed)
                throw new InvalidOperationException("Double dispose.");

            Disposed = true;
        }
    }
}
