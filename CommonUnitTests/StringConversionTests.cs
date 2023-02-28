// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Moq;
using System;
using System.IO;
using Xunit;
using Yextly.Common;

namespace CommonUnitTests
{
    public sealed class OwnershipTests
    {
        [Fact]
        public void CanPreventDisposalOnNonOwnedObjects()
        {
            var instance = new Mock<IDisposable>();

            instance
                .Setup(x => x.Dispose())
                .Throws<InvalidOperationException>();

            using var d = instance.Object.AsNonOwned();
        }

        [Fact]
        public void CanPreventDisposalOnStreams()
        {
            var stream = new FailingStream();

            using var s = new UnclosableStream(stream);
        }

        private sealed class FailingStream : MemoryStream
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2215:Dispose methods should call base class dispose", Justification = "This is exactly what we are testing")]
            protected override void Dispose(bool disposing)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
