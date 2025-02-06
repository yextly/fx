// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;
using System.Reflection;

namespace Yextly.Scripting.Services.SingleFileApplication
{
    /// <summary>
    /// Contains information about the types in the scripting engine.
    /// </summary>
    public sealed record ScriptingRuntimeTypeInfo
    {
        /// <summary>
        /// Contains the list of consolidated assemblies that must be referenced.
        /// </summary>
        public required ImmutableArray<Assembly> Assemblies { get; init; }

        /// <summary>
        /// Contains the list of all the consolidated known types.
        /// </summary>
        public required ImmutableArray<Type> KnownTypes { get; init; }

        /// <summary>
        /// Contains the list of all the consolidated hardcoded using directive to inject.
        /// </summary>
        public required ImmutableArray<string> Usings { get; init; }
    }
}
