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

            // this is a patch to preserve calls to CreateScript when using references only
            if (metadataReferences.Length == 0 && !References.Contains(typeof(object).Assembly))
            {
                // this is a temporary workaround, before we remove the assembly references.
                // when you specify the metadata references, you are expected to specify the "object" assembly, so we don't do it here
                references = references.Add(typeof(object).Assembly);

                var runtime = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), x => string.Equals("System.Runtime", x.GetName().Name, StringComparison.Ordinal));

                if (runtime != null)
                    references = references.Add(runtime);

                if (HostInstanceType != null && !References.Contains(HostInstanceType.Assembly))
                    references = references.Add(HostInstanceType.Assembly);
                if (HostInstance != null && !References.Contains(HostInstance.GetType().Assembly))
                {
                    var temp = HostInstance.GetType().Assembly;
                    if (HostInstanceType == null || HostInstanceType.Assembly != temp)
                        references = references.Add(temp);
                }
            }

            return new ExecutionContext(references, metadataReferences, usings, HostInstance, HostInstanceType);
        }
    }
}
