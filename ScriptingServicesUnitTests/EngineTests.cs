// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Threading.Tasks;
using Xunit;
using Yextly.Scripting.Abstractions;
using Yextly.Scripting.Services;

namespace ScriptingServicesTests
{
    public sealed class EngineTests

    {
        [Fact]
        public void CanBuildTheEngine()
        {
            Assert.NotNull(ScriptingServicesFactories.ServiceFactories);
        }

        [InlineData(new object[] { "int i = 40; return i + 2;", 42 })]
        [InlineData(new object[] { "40 + 2 ", 42 })]
        [Theory]
        public async Task CanBuildTrivialScripts(string text, int expected)
        {
            var factory = ScriptingServicesFactories.ServiceFactories;

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

        [InlineData(new object[] { "int i = 2; return Sum(i, 20);", 42 })]
        [InlineData(new object[] { "Sum(20, 2)", 42 })]
        [Theory]
        public async Task CanBuildTrivialScriptsWithHostInstance(string text, int expected)
        {
            var factory = ScriptingServicesFactories.ServiceFactories;

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

        [InlineData(new object[] { "HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42 })]
        [Theory]
        public async Task CanBuildTrivialScriptsWithSeparateAssembliesAndUsings(string text, int expected)
        {
            var factory = ScriptingServicesFactories.ServiceFactories;

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

        [InlineData(new object[] { "HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42 })]
        [InlineData(new object[] { "int useless = 5; HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3", 42 })]
        [InlineData(new object[] { "int useless = 5; return HostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3;", 42 })]
        [Theory]
        public async Task CanBuildTrivialScriptsWithSeparateAssembliesAndUsingsWithInference(string text, int expected)
        {
            var factory = ScriptingServicesFactories.ServiceFactories;

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

        [InlineData(new object[] { "4;5;6;" })]
        [InlineData(new object[] { "4;5;6" })]
        [InlineData(new object[] { "hostOperations.Multiply(HostOperations.Sum(5, 10), 3)-3" })]
        [Theory]
        public void CanDetectErrorsWhenBuildingTrivialScriptsWithSeparateAssembliesAndUsingsWithInference(string text)
        {
            var factory = ScriptingServicesFactories.ServiceFactories;

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

        [InlineData(new object[] { "int i = 2; return Sum(i, 20)" })]
        [InlineData(new object[] { "Sum(20, 2))" })]
        [InlineData(new object[] { "sum(20, 2)" })]
        [InlineData(new object[] { "Math.Round(20, 2)" })]
        [Theory]
        public void CanDetectTrivialScriptErrors(string text)
        {
            var factory = ScriptingServicesFactories.ServiceFactories;

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

        [Fact]
        public async Task CanRunAScriptWithSideEffectsReturingVoid()
        {
            const string text = @"var a = 15;
                var b = 20;
                SideEffect1(a + b);
                var c = b - a;
                SideEffect2(c);
";

            var factory = ScriptingServicesFactories.ServiceFactories;

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
    }
}
