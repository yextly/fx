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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S3877:Exceptions should not be thrown from unexpected methods", Justification = "This is a test")]
        public void Dispose()
        {
            if (Disposed)
            {
                throw new InvalidOperationException("Double dispose.");
            }

            Disposed = true;
        }
    }
}
