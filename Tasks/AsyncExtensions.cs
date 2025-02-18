// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Tasks
{
    /// <summary>
    /// Contains general purpose asynchronous extensions.
    /// </summary>
    public static class AsyncExtensions
    {
        /// <summary>
        /// Turns an asynchronous <see cref="Task{T}"/> into a synchronous one.
        /// </summary>
        /// <typeparam name="T">The type of the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static T AsSync<T>(this Task<T> task)
        {
            ArgumentNullException.ThrowIfNull(task);

            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Turns an asynchronous <see cref="Task"/> into a synchronous one.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static void AsSync(this Task task)
        {
            ArgumentNullException.ThrowIfNull(task);

            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
