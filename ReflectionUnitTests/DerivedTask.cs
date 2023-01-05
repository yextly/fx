// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionUnitTests
{
    internal sealed class DerivedTask<T> : Task<T>
    {
        public DerivedTask(Func<T> function) : base(function)
        {
        }

        public DerivedTask(Func<object?, T> function, object? state) : base(function, state)
        {
        }

        public DerivedTask(Func<T> function, CancellationToken cancellationToken) : base(function, cancellationToken)
        {
        }

        public DerivedTask(Func<T> function, TaskCreationOptions creationOptions) : base(function, creationOptions)
        {
        }

        public DerivedTask(Func<object?, T> function, object? state, CancellationToken cancellationToken) : base(function, state, cancellationToken)
        {
        }

        public DerivedTask(Func<object?, T> function, object? state, TaskCreationOptions creationOptions) : base(function, state, creationOptions)
        {
        }
    }
}
