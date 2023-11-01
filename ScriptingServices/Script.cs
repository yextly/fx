// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis.Scripting;
using Yextly.Scripting.Abstractions;

namespace Yextly.Scripting.Services
{
    internal sealed class Script : IScript
    {
        public Script(Guid id, IScriptingExecutionContext executionContext, ScriptRunner<object> runner, string text)
        {
            ArgumentNullException.ThrowIfNull(executionContext);
            ArgumentNullException.ThrowIfNull(runner);
            ArgumentNullException.ThrowIfNull(text);

            Id = id;
            ExecutionContext = executionContext;
            Runner = runner;
            Text = text;
        }

        public IScriptingExecutionContext ExecutionContext { get; }
        public Guid Id { get; }

        public ScriptRunner<object> Runner { get; }
        public string Text { get; }
    }
}
