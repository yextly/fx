// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using System.Globalization;
using System.Reflection;
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
                    .AddReferences(executionContext.MetadataReferences)
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

        public IScript CreateScriptForSingleFileApplication(IScriptingExecutionContext executionContext, string text)
        {
            ArgumentNullException.ThrowIfNull(executionContext);

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var id = Guid.NewGuid();

            var assemblyName = "ScriptEngine" + id.ToString("N", CultureInfo.InvariantCulture);

            var diagnostics = new List<Diagnostic>();

            bool catchExceptions = true;

            try
            {
                var parseOptions = new CSharpParseOptions(languageVersion: LanguageVersion.Latest, kind: SourceCodeKind.Script);

                var additionalSyntaxTrees = new List<SyntaxTree>();

                var syntaxTree = SyntaxFactory.ParseSyntaxTree(text, parseOptions);

                diagnostics.AddRange(syntaxTree.GetDiagnostics());

                IEnumerable<MetadataReference> references = executionContext.MetadataReferences;

                if (executionContext.References.Length > 0)
                {
                    // this will fail on a real SFA, but since you may be calling this api for consistency,
                    // we emulate the broken behaviour.
                    references = references
                        .Concat(
                            executionContext.References
                                .Select(x => MetadataReference.CreateFromFile(x.Location))
                        );
                }

                var csharpOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable)
                    .WithUsings(executionContext.Usings);

                var compilation = CSharpCompilation.CreateScriptCompilation(
                    assemblyName: assemblyName,
                    syntaxTree,
                    references,
                    csharpOptions,
                    previousScriptCompilation: null,
                    returnType: typeof(object),
                    globalsType: executionContext.HostInstanceType
                );

                var methodInvoker = default(Func<object?[], Task<object>>);

                var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb, pdbChecksumAlgorithm: default);

                using (var peStream = new MemoryStream())
                using (var pdbStream = new MemoryStream())
                {
                    var result = compilation.Emit(peStream, pdbStream, xmlDocumentationStream: null, win32Resources: null, manifestResources: null, emitOptions);

                    diagnostics.AddRange(result.Diagnostics);

                    if (result.Success)
                    {
                        var scriptAssembly = AppDomain.CurrentDomain.Load(peStream.ToArray(), pdbStream.ToArray());

                        var entryPoint = compilation.GetEntryPoint(CancellationToken.None) ?? throw new InvalidOperationException("Cannot find the entry point.");

                        var entryPointType = scriptAssembly
                            .GetType(
                                $"{entryPoint.ContainingNamespace.MetadataName}.{entryPoint.ContainingType.MetadataName}",
                                throwOnError: true,
                                ignoreCase: false);

                        var entryPointMethod = entryPointType?.GetTypeInfo().GetDeclaredMethod(entryPoint.MetadataName) ?? throw new InvalidOperationException("Cannot find the entry point method in the compiled assembly.");

                        methodInvoker = entryPointMethod.CreateDelegate<Func<object?[], Task<object>>>();
                    }
                }

                if (methodInvoker == null)
                {
                    var ex = new InvalidSourceTextException("The provided text cannot be compiled due to syntax errors.");
                    ex.Data.Add("Diagnostics", diagnostics);

                    catchExceptions = false;
                    throw ex;
                }

                return new AssemblyBackedScript(id, executionContext, methodInvoker, text);
            }
            catch (Exception ex) when (catchExceptions)
            {
                throw new InvalidSourceTextException("The provided text cannot be compiled into executable code.", ex);
            }
        }

        public async Task<object?> RunScriptAsync(IScript script, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(script);

            if (script is Script roslynScript)
                return await ScriptingEngine.RunScriptInternalAsync(roslynScript, cancellationToken).ConfigureAwait(false);
            else if (script is AssemblyBackedScript assemblyBackedScript)
                return await ScriptingEngine.RunScriptInternalAsync(assemblyBackedScript, cancellationToken).ConfigureAwait(false);
            else
                throw new NotSupportedException("Unsupported script type.");
        }

        private static async Task<object?> RunScriptInternalAsync(Script script, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(script);

            try
            {
                var task = (Task<object>)script.Runner(script.ExecutionContext.HostInstance, cancellationToken);

                return await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new RuntimeExecutionException("The script cannot be executed.", ex);
            }
        }

        private static async Task<object?> RunScriptInternalAsync(AssemblyBackedScript script, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(script);

            try
            {
                // in this very moment, we don't support cancellation. however, we try to pass it as second parameter (which will be probably ignored)
                var task = (Task<object>)script.Runner([script.ExecutionContext.HostInstance, cancellationToken]);

                cancellationToken.ThrowIfCancellationRequested();

                return await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new RuntimeExecutionException("The script cannot be executed.", ex);
            }
        }
    }
}
