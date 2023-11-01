// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Contains the factories needed to interact with the scripting engine.
    /// </summary>
    public interface IScriptingEngineFactories
    {
        /// <summary>
        /// Creates a new engine.
        /// </summary>
        /// <returns></returns>
        IScriptingEngine CreateEngine();

        /// <summary>
        /// Creates a new execution context builder.
        /// </summary>
        /// <returns></returns>
        IScriptingExecutionContextBuilder CreateScriptingExecutionContextBuilder();
    }
}
