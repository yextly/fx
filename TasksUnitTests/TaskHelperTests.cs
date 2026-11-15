// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Yextly.Tasks;

namespace TasksUnitTests
{
    public sealed class TaskHelperTests
    {
        [Fact]
        public async Task YieldAsyncCompletes()
        {
            await TaskHelper.YieldAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        }

        [Fact]
        public async Task YieldAsyncThrowsWhenCancelled()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            await cancellationTokenSource.CancelAsync().ConfigureAwait(true);

            await Assert.ThrowsAsync<OperationCanceledException>(() => TaskHelper.YieldAsync(cancellationTokenSource.Token)).ConfigureAwait(true);
        }
    }
}
