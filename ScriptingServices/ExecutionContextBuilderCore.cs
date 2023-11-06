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
    internal sealed class ExecutionContextBuilder : IScriptingExecutionContextBuilder
    {
        public IList<Type> AutoIncludedTypes { get; } = new List<Type>();
        public object? HostInstance { get; set; }
        public Type? HostInstanceType { get; set; }
        public IList<MetadataReference> MetadataReferences { get; } = new List<MetadataReference>();
        public IList<Assembly> References { get; } = new List<Assembly>();
        public IList<string> Usings { get; } = new List<string>();

        public IScriptingExecutionContext Build()
        {
            foreach (var item in AutoIncludedTypes)
            {
                References.Add(item.Assembly);

                if (item.Namespace != null)
                    Usings.Add(item.Namespace);
            }

            var references = References
                .Distinct()
                .ToImmutableArray();

            var metadataReferences = MetadataReferences
                .Distinct()
                .ToImmutableArray();

            var usings = Usings
                .Distinct(StringComparer.Ordinal)
                .ToImmutableArray();

            return new ExecutionContext(references, metadataReferences, usings, HostInstance, HostInstanceType);
        }
    }
}
