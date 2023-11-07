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
        /// Creates a new script when hosted in a Single File Application.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <param name="text">The text that must be compiled to a script.</param>
        /// <returns></returns>
        /// <remarks>This is a workaround method for https://github.com/dotnet/roslyn/issues/50719 and a serious side effect of polluting the domain with multiple assmblies.
        /// Additionally, since we won't add unsafe code here, you need to provide the metadata for `typeof(object).Assembly`.
        /// </remarks>
        IScript CreateScriptForSingleFileApplication(IScriptingExecutionContext executionContext, string text);

        /// <summary>
        /// Runs a script.
        /// </summary>
        /// <param name="script">The script to run.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<object?> RunScriptAsync(IScript script, CancellationToken cancellationToken = default);
    }
}
