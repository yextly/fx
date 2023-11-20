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
    internal sealed class ExecutionContext(ImmutableArray<Assembly> references, ImmutableArray<MetadataReference> metadataReferences, ImmutableArray<string> usings, object? hostInstance, Type? hostInstanceType) : IScriptingExecutionContext
    {
        public object? HostInstance { get; } = hostInstance;
        public Type? HostInstanceType { get; } = hostInstanceType;
        public ImmutableArray<MetadataReference> MetadataReferences { get; } = metadataReferences;
        public ImmutableArray<Assembly> References { get; } = references;
        public ImmutableArray<string> Usings { get; } = usings;
    }
}
