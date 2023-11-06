// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Threading;
using System.Threading.Tasks;
using Yextly.Scripting.Abstractions;

namespace ScriptingServicesTests
{
    internal sealed class MultiplexedScriptingEngine : IScriptingEngine
    {
        private readonly IScriptingEngine _inner;
        private readonly ScriptType _scriptType;

        public MultiplexedScriptingEngine(IScriptingEngine inner, ScriptType scriptType)
        {
            _inner = inner;
            _scriptType = scriptType;
        }

        public IScript CreateScript(IScriptingExecutionContext executionContext, string text)
        {
            // this is the one we want to multiplex

            return _scriptType switch
            {
                ScriptType.Roslyn => _inner.CreateScript(executionContext, text),
                ScriptType.SFA => _inner.CreateScriptForSingleFileApplication(executionContext, text),
                _ => throw new NotSupportedException(),
            };
        }

        public IScript CreateScriptForSingleFileApplication(IScriptingExecutionContext executionContext, string text)
        {
            return _inner.CreateScriptForSingleFileApplication(executionContext, text);
        }

        public Task<object?> RunScriptAsync(IScript script, CancellationToken cancellationToken = default)
        {
            return _inner.RunScriptAsync(script, cancellationToken);
        }
    }
}
