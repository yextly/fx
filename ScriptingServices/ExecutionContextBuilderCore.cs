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
    internal sealed class ExecutionContextBuilder : IScriptingExecutionContextBuilder
    {
        public IList<Type> AutoIncludedTypes { get; } = new List<Type>();
        public object? HostInstance { get; set; }
        public Type? HostInstanceType { get; set; }
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

            return new ExecutionContext(References.Distinct().ToImmutableArray(), Usings.Distinct().ToImmutableArray(), HostInstance, HostInstanceType);
        }
    }
}
