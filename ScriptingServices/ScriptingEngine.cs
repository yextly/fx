// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Yextly.Scripting.Abstractions;

namespace Yextly.Scripting.Services
{
    internal sealed class ScriptingEngine : IScriptingEngine
    {
        public IScript CreateScript(IScriptingExecutionContext executionContext, string text)
        {
            ArgumentNullException.ThrowIfNull(executionContext);

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            try
            {
                var options = ScriptOptions.Default
                    .AddReferences(executionContext.References)
                    .AddImports(executionContext.Usings)
                    .WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Release)
                    ;

                var script = CSharpScript.Create(text, options, executionContext.HostInstanceType ?? executionContext.HostInstance?.GetType(), null);

                var runner = script.CreateDelegate();

                return new Script(Guid.NewGuid(), executionContext, runner, text);
            }
            catch (Exception ex)
            {
                throw new InvalidSourceTextException("The provided text cannot be compiled into executable code.", ex);
            }
        }

        public async Task<object?> RunScriptAsync(IScript script, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(script);

            if (script is not Script descriptor)
                throw new InvalidOperationException();

            try
            {
                var task = (Task<object>)descriptor.Runner(descriptor.ExecutionContext.HostInstance, cancellationToken);

                return await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new RuntimeExecutionException("The script cannot be executed.", ex);
            }
        }
    }
}
