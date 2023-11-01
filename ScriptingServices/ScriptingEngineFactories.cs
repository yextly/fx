// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Scripting.Abstractions;

namespace Yextly.Scripting.Services
{
    internal sealed class ScriptingEngineFactories : IScriptingEngineFactories
    {
        public IScriptingEngine CreateEngine()
        {
            return new ScriptingEngine();
        }

        public IScriptingExecutionContextBuilder CreateScriptingExecutionContextBuilder()
        {
            return new ExecutionContextBuilder();
        }
    }
}
