// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Scripting.Abstractions;
using Yextly.Scripting.Services;

namespace ScriptingServicesTests
{
    internal sealed class MultiplexedScriptingEngineFactories(ScriptType scriptType) : IScriptingEngineFactories
    {
        private readonly IScriptingEngineFactories _factories = ScriptingServicesFactories.ServiceFactories;
        private readonly ScriptType _scriptType = scriptType;

        public IScriptingEngine CreateEngine()
        {
            return new MultiplexedScriptingEngine(_factories.CreateEngine(), _scriptType);
        }

        public IScriptingExecutionContextBuilder CreateScriptingExecutionContextBuilder()
        {
            return _factories.CreateScriptingExecutionContextBuilder();
        }
    }
}
