// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Represents the scripting engine.
    /// </summary>
    /// <remarks>The members of this instance are thread safe.</remarks>
    public interface IScriptingEngine
    {
        /// <summary>
        /// Creates a new script.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <param name="text">The text that must be compiled to a script.</param>
        /// <returns></returns>
        IScript CreateScript(IScriptingExecutionContext executionContext, string text);

        /// <summary>
        /// Runs a script.
        /// </summary>
        /// <param name="script">The script to run.</param>
        /// <returns></returns>
        object? RunScript(IScript script);
    }
}
