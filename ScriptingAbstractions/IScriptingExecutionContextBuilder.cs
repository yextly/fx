// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis;
using System.Reflection;

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Represents a scripting execution context builder.
    /// </summary>
    public interface IScriptingExecutionContextBuilder
    {
        /// <summary>
        /// Contains the list of types that should be immediately available to the script.
        /// </summary>
        /// <remarks>Assemblies and namespaces are automatically inferred.</remarks>
        IList<Type> AutoIncludedTypes { get; }

        /// <summary>
        /// Host entry point instance used for providing the static context.
        /// </summary>
        object? HostInstance { get; set; }

        /// <summary>
        /// The type of the host instance.
        /// </summary>
        /// <remarks>The type is automatically inferred by the provided instance. You need to use this property to have a different runtime types be reachable to the script.</remarks>
        Type? HostInstanceType { get; set; }

        /// <summary>
        /// Contains the referenced assemblies as metadata.
        /// </summary>
        IList<MetadataReference> MetadataReferences { get; }

        /// <summary>
        /// Contains the referenced assemblies to use. This is a shortcut for <see cref="MetadataReferences"/>, since the engine will perform metadata inference from the provided assemblies.
        /// </summary>
        /// <remarks>Metadata inference use <see cref="Assembly.Location"/>.</remarks>
        IList<Assembly> References { get; }

        /// <summary>
        /// Contains the using clauses to use.
        /// </summary>
        IList<string> Usings { get; }

        /// <summary>
        /// Creates the evaluation context.
        /// </summary>
        /// <returns></returns>
        IScriptingExecutionContext Build();
    }
}
