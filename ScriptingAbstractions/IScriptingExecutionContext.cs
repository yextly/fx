// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Represents a scripting execution context.
    /// </summary>
    public interface IScriptingExecutionContext
    {
        /// <summary>
        /// Host entry point instance reachable from the scripting engine. When used, it supplies the "this" instance.
        /// </summary>
        object? HostInstance { get; }

        /// <summary>
        /// The type of <see cref="HostInstance"/>.
        /// </summary>
        /// <remarks>The type is automatically inferred by the instance passed when bulding the context.</remarks>
        Type? HostInstanceType { get; }

        /// <summary>
        /// Specifies the list of referenced assemblies.
        /// </summary>
        ImmutableArray<MetadataReference> MetadataReferences { get; }

        /// <summary>
        /// Specifies the list of referenced assemblies.
        /// </summary>
        ImmutableArray<Assembly> References { get; }

        /// <summary>
        /// Specifies namespaces to be imported.
        /// </summary>
        ImmutableArray<string> Usings { get; }
    }
}
