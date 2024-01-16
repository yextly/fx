// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Concurrent;

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class OperationChain
    {
        private readonly ConcurrentQueue<OperationFlow> _operations = new();

        public OperationChain Clone()
        {
            var ret = new OperationChain();
            foreach (var item in _operations)
            {
                ret.Enqueue(item);
            }

            return ret;
        }

        public OperationFlow? DequeueNext()
        {
            if (_operations.TryDequeue(out var flow))
                return flow;
            else
                return null;
        }

        public void Enqueue(OperationFlow operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            _operations.Enqueue(operation);
        }
    }
}
