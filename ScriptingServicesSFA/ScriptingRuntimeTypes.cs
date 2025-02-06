// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Services.SingleFileApplication
{
    /// <summary>
    /// Contains the collection of all the types used both for analysis and compilation by the scripting engine.
    /// </summary>
    public sealed record ScriptingRuntimeTypes
    {
        /// <summary>
        /// Contains the analysis-time types.
        /// </summary>
        public required ScriptingRuntimeTypeInfo AnalysisTypes { get; init; }

        /// <summary>
        /// Contains the compilation-time types.
        /// </summary>
        public required ScriptingRuntimeTypeInfo CompilationTypes { get; init; }
    }
}
