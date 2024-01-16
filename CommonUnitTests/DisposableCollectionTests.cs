// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;
using Yextly.Common;

namespace CommonUnitTests
{
    public sealed class DisposableCollectionTests
    {
        private readonly List<DisposableObject> _source;

        public DisposableCollectionTests()
        {
            var source = new List<DisposableObject>();

            for (int i = 0; i < 10; i++)
            {
                source.Add(new DisposableObject());
            }

            Assert.All(source, x => Assert.False(x.Disposed));

            _source = source;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanDisposeACollection(bool safe)
        {
            var original = new List<DisposableObject>(_source);

            ObjectStealer<IDisposableCollection<DisposableObject>> stealer;

            using (var collection = original.AsDisposableCollection(safe))
            {
                Assert.Equal(original.Count, collection.Count);
                Assert.Equal(_source.Count, collection.Count);

                stealer = collection.Steal();
            }

            Assert.Empty(original);
            Assert.All(_source, x => Assert.True(x.Disposed));

            Assert.ThrowsAny<ObjectDisposedException>(() => _ = stealer.Value.Count);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanDisposeAConcurrentBag(bool safe)
        {
            var original = new ConcurrentBag<DisposableObject>(_source);

            ObjectStealer<IDisposableProducerConsumerCollection<DisposableObject>> stealer;

            using (var collection = original.AsDisposableCollection(safe))
            {
                Assert.Equal(original.Count, collection.Count);
                Assert.Equal(_source.Count, collection.Count);

                stealer = collection.Steal();
            }

            Assert.Empty(original);
            Assert.All(_source, x => Assert.True(x.Disposed));

            Assert.ThrowsAny<ObjectDisposedException>(() => _ = stealer.Value.Count);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanDisposeAList(bool safe)
        {
            var original = new List<DisposableObject>(_source);

            ObjectStealer<IDisposableList<DisposableObject>> stealer;

            using (var collection = original.AsDisposableList(safe))
            {
                Assert.Equal(original.Count, collection.Count);
                Assert.Equal(_source.Count, collection.Count);

                stealer = collection.Steal();
            }

            Assert.Empty(original);
            Assert.All(_source, x => Assert.True(x.Disposed));

            Assert.ThrowsAny<ObjectDisposedException>(() => _ = stealer.Value.Count);
        }
    }
}
