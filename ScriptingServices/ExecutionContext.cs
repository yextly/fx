// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;
using Yextly.Scripting.Abstractions;

namespace Yextly.Scripting.Services
{
    internal sealed class ExecutionContext : IScriptingExecutionContext
    {
        public ExecutionContext(ImmutableArray<Assembly> references, ImmutableArray<MetadataReference> metadataReferences, ImmutableArray<string> usings, object? hostInstance, Type? hostInstanceType)
        {
            References = references;
            MetadataReferences = metadataReferences;
            Usings = usings;
            HostInstance = hostInstance;
            HostInstanceType = hostInstanceType;
        }

        public object? HostInstance { get; }
        public Type? HostInstanceType { get; }
        public ImmutableArray<MetadataReference> MetadataReferences { get; }
        public ImmutableArray<Assembly> References { get; }
        public ImmutableArray<string> Usings { get; }
    }
}
