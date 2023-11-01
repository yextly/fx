// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;
using System.Reflection;
using Yextly.Scripting.Abstractions;

namespace Yextly.Scripting.Services
{
    internal sealed class ExecutionContext : IScriptingExecutionContext
    {
        public ExecutionContext(ImmutableArray<Assembly> references, ImmutableArray<string> usings, object? hostInstance, Type? hostInstanceType)
        {
            References = references;
            Usings = usings;
            HostInstance = hostInstance;
            HostInstanceType = hostInstanceType;
        }

        public object? HostInstance { get; }
        public Type? HostInstanceType { get; }
        public ImmutableArray<Assembly> References { get; }
        public ImmutableArray<string> Usings { get; }
    }
}
