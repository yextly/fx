// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Scripting.Abstractions;
using Yextly.Scripting.Services;

namespace ScriptingServicesTests
{
    internal sealed class MultiplexedScriptingEngineFactories : IScriptingEngineFactories
    {
        private readonly IScriptingEngineFactories _factories;
        private readonly ScriptType _scriptType;

        public MultiplexedScriptingEngineFactories(ScriptType scriptType)
        {
            _factories = ScriptingServicesFactories.ServiceFactories;
            _scriptType = scriptType;
        }

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
