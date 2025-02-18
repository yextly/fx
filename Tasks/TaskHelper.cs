// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Tasks
{
    /// <summary>
    /// Provides helper functions for <see cref="Task"/>.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// Reschedules the current asynchronous flow in another task.
        /// </summary>
        /// <returns></returns>
        /// <remarks>The purpose of this method is to wrap Task.Yield so that each asynchronous calls follow the recommended patterns.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Async/await", "CRR0029:ConfigureAwait(true) is called implicitly", Justification = "False positive")]
        public static async Task YieldAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
        }
    }
}
