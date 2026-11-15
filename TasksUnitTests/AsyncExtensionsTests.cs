// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Threading.Tasks;
using Xunit;
using Yextly.Tasks;

namespace TasksUnitTests
{
    public sealed class AsyncExtensionsTests
    {
        [Fact]
        public void AsSyncReturnsTheTaskResult()
        {
            var actual = Task.FromResult(42).AsSync();

            Assert.Equal(42, actual);
        }

        [Fact]
        public void AsSyncWaitsForTheTask()
        {
            var task = Task.Delay(10);

            task.AsSync();

            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public void AsSyncRethrowsTheOriginalException()
        {
            var task = Task.FromException<int>(new InvalidOperationException("failure"));

            var exception = Assert.Throws<InvalidOperationException>(() => task.AsSync());

            Assert.Equal("failure", exception.Message);
        }

        [Fact]
        public void AsSyncRejectsANullTask()
        {
            Assert.Throws<ArgumentNullException>(() => ((Task)null!).AsSync());
            Assert.Throws<ArgumentNullException>(() => ((Task<int>)null!).AsSync());
        }
    }
}
