// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Yextly.Scripting.Abstractions;

namespace ScriptingServicesTests
{
    public sealed class EngineTests
    {
        [InlineData(ScriptType.Roslyn)]
        [InlineData(ScriptType.SFA)]
        [Theory]
        public void CanBuildTheEngine(ScriptType scriptType)
        {
            Assert.NotNull(GetFactories(scriptType));
        }

        [InlineData("int i = 40; return i + 2;", 42, ScriptType.SFA)]
        [InlineData("40 + 2 ", 42, ScriptType.SFA)]
        [InlineData("int i = 40; return i + 2;", 42, ScriptType.Roslyn)]
        [InlineData("40 + 2 ", 42, ScriptType.Roslyn)]
        [Theory]
        public async Task CanBuildTrivialScripts(string text, int expected, ScriptType scriptType)
        {
            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            var e = engine.CreateScript(context, text);
            Assert.NotNull(e);

            var result = await engine.RunScriptAsync(e).ConfigureAwait(true);

            Assert.IsType<int>(result);
            Assert.Equal(expected, (int)result!);
        }

        [InlineData("int i = 2; return Sum(i, 20);", 42, ScriptType.Roslyn)]
        [InlineData("Sum(20, 2)", 42, ScriptType.Roslyn)]
        [InlineData("int i = 2; return Sum(i, 20);", 42, ScriptType.SFA)]
        [InlineData("Sum(20, 2)", 42, ScriptType.SFA)]
        [Theory]
        public async Task CanBuildTrivialScriptsWithHostInstance(string text, int expected, ScriptType scriptType)
        {
            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var globalContext = new Context1(20);

            builder.HostInstance = globalContext;
            builder.HostInstanceType = typeof(IContext1);

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            var e = engine.CreateScript(context, text);
            Assert.NotNull(e);

            var result = await engine.RunScriptAsync(e).ConfigureAwait(true);

            Assert.IsType<int>(result);
            Assert.Equal(expected, (int)result!);
        }

        [InlineData("HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.Roslyn)]
        [InlineData("HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.SFA)]
        [Theory]
        public async Task CanBuildTrivialScriptsWithSeparateAssembliesAndUsings(string text, int expected, ScriptType scriptType)
        {
            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var globalContext = new Context1(20);

            builder.HostInstance = globalContext;
            builder.HostInstanceType = typeof(IContext1);
            builder.References.Add(typeof(Yextly.Scripting.Testing.Assembly1.HostOperations).Assembly);
            builder.Usings.Add(typeof(Yextly.Scripting.Testing.Assembly1.HostOperations).Namespace!);

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            var e = engine.CreateScript(context, text);
            Assert.NotNull(e);

            var result = await engine.RunScriptAsync(e).ConfigureAwait(true);

            Assert.IsType<int>(result);
            Assert.Equal(expected, (int)result!);
        }

        [InlineData("HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.Roslyn)]
        [InlineData("int useless = 5; HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.Roslyn)]
        [InlineData("int useless = 5; return HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3;", 42, ScriptType.Roslyn)]
        [InlineData("HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.SFA)]
        [InlineData("int useless = 5; HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.SFA)]
        [InlineData("int useless = 5; return HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3;", 42, ScriptType.SFA)]
        [Theory]
        public async Task CanBuildTrivialScriptsWithSeparateAssembliesAndUsingsWithInference(string text, int expected, ScriptType scriptType)
        {
            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var globalContext = new Context1(20);

            builder.HostInstance = globalContext;
            builder.HostInstanceType = typeof(IContext1);
            builder.AutoIncludedTypes.Add(typeof(Yextly.Scripting.Testing.Assembly1.HostOperations));

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            var e = engine.CreateScript(context, text);
            Assert.NotNull(e);

            var result = await engine.RunScriptAsync(e).ConfigureAwait(true);

            Assert.IsType<int>(result);
            Assert.Equal(expected, (int)result!);
        }

        [InlineData("HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.Roslyn)]
        [InlineData("HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42, ScriptType.SFA)]
        [Theory]
        public async Task CanBuildTrivialScriptsWithSeparateAssembliesWithMetadataAndUsings(string text, int expected, ScriptType scriptType)
        {
            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var globalContext = new Context1(20);

            builder.HostInstance = globalContext;
            builder.HostInstanceType = typeof(IContext1);
            builder.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            builder.MetadataReferences.Add(MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().Single(x => x.FullName?.StartsWith("System.Runtime,", StringComparison.Ordinal) == true).Location));
            builder.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(Yextly.Scripting.Testing.Assembly1.HostOperations).Assembly.Location));
            builder.Usings.Add(typeof(Yextly.Scripting.Testing.Assembly1.HostOperations).Namespace!);

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            var e = engine.CreateScript(context, text);
            Assert.NotNull(e);

            var result = await engine.RunScriptAsync(e).ConfigureAwait(true);

            Assert.IsType<int>(result);
            Assert.Equal(expected, (int)result!);
        }

        [InlineData("4;5;6;", ScriptType.Roslyn)]
        [InlineData("4;5;6", ScriptType.Roslyn)]
        [InlineData("hostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", ScriptType.Roslyn)]
        [InlineData("4;5;6;", ScriptType.SFA)]
        [InlineData("4;5;6", ScriptType.SFA)]
        [InlineData("hostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", ScriptType.SFA)]
        [Theory]
        public void CanDetectErrorsWhenBuildingTrivialScriptsWithSeparateAssembliesAndUsingsWithInference(string text, ScriptType scriptType)
        {
            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var globalContext = new Context1(20);

            builder.HostInstance = globalContext;
            builder.HostInstanceType = typeof(IContext1);
            builder.AutoIncludedTypes.Add(typeof(Yextly.Scripting.Testing.Assembly1.HostOperations));

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            Assert.Throws<InvalidSourceTextException>(() => engine.CreateScript(context, text));
        }

        [InlineData("int i = 2; return Sum(i, 20)", ScriptType.Roslyn)]
        [InlineData("Sum(20, 2))", ScriptType.Roslyn)]
        [InlineData("sum(20, 2)", ScriptType.Roslyn)]
        [InlineData("Math.Round(20, 2)", ScriptType.Roslyn)]
        [InlineData("int i = 2; return Sum(i, 20)", ScriptType.SFA)]
        [InlineData("Sum(20, 2))", ScriptType.SFA)]
        [InlineData("sum(20, 2)", ScriptType.SFA)]
        [InlineData("Math.Round(20, 2)", ScriptType.SFA)]
        [Theory]
        public void CanDetectTrivialScriptErrors(string text, ScriptType scriptType)
        {
            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var globalContext = new Context1(20);

            builder.HostInstance = globalContext;
            builder.HostInstanceType = typeof(IContext1);

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            Assert.Throws<InvalidSourceTextException>(() => engine.CreateScript(context, text));
        }

        [InlineData(ScriptType.Roslyn)]
        [InlineData(ScriptType.SFA)]
        [Theory]
        public async Task CanRunAScriptWithSideEffectsReturingVoid(ScriptType scriptType)
        {
            const string text = @"var a = 15;
                var b = 20;
                SideEffect1(a + b);
                var c = b - a;
                SideEffect2(c);
";

            var factory = GetFactories(scriptType);

            var builder = factory.CreateScriptingExecutionContextBuilder();

            var globalContext = new Context2();

            builder.HostInstance = globalContext;
            builder.HostInstanceType = typeof(IContext2);
            builder.AutoIncludedTypes.Add(typeof(Yextly.Scripting.Testing.Assembly1.HostOperations));

            var context = builder.Build();
            Assert.NotNull(context);

            var engine = factory.CreateEngine();
            Assert.NotNull(engine);

            var e = engine.CreateScript(context, text);
            Assert.NotNull(e);

            var result = await engine.RunScriptAsync(e).ConfigureAwait(true);

            Assert.Null(result);

            Assert.Equal(85000, globalContext.Value);
        }

        private static IScriptingEngineFactories GetFactories(ScriptType scriptType)
        {
            return new MultiplexedScriptingEngineFactories(scriptType);
        }
    }
}
