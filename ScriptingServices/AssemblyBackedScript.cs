// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Scripting.Abstractions;

namespace Yextly.Scripting.Services
{
    internal sealed class AssemblyBackedScript : IScript
    {
        public AssemblyBackedScript(Guid id, IScriptingExecutionContext executionContext, Func<object?[], Task<object>> runner, string text)
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

        public Func<object?[], Task<object>> Runner { get; }
        public string Text { get; }
    }
}
