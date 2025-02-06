// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Scripting.Services.SingleFileApplication;

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Provides extensions for <see cref="IScriptingExecutionContextBuilder"/>.
    /// </summary>
    public static class ScriptingExecutionContextBuilderExtensions
    {
        /// <summary>
        /// Adds the analysis-time types to the provided <see cref="IScriptingExecutionContextBuilder" />.
        /// </summary>
        /// <param name="builder">The builder receiving the type references.</param>
        /// <param name="types">The scripting metadata descriptors.</param>
        /// <returns></returns>
        public static IScriptingExecutionContextBuilder AddAnalysisTypes(this IScriptingExecutionContextBuilder builder, ScriptingRuntimeTypes types)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(types);

            var source = types.AnalysisTypes;

            AddTypes(builder, source);

            return builder;
        }

        /// <summary>
        /// Adds the compile-time types to the provided <see cref="IScriptingExecutionContextBuilder" />.
        /// </summary>
        /// <param name="builder">The builder receiving the type references.</param>
        /// <param name="types">The scripting metadata descriptors.</param>
        /// <returns></returns>
        public static IScriptingExecutionContextBuilder AddCompilationTypes(this IScriptingExecutionContextBuilder builder, ScriptingRuntimeTypes types)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(types);

            var source = types.CompilationTypes;

            AddTypes(builder, source);

            return builder;
        }

        private static void AddTypes(IScriptingExecutionContextBuilder builder, ScriptingRuntimeTypeInfo source)
        {
            foreach (var item in source.Assemblies)
            {
                builder.MetadataReferences.Add(MetadataReferenceCreator.CreateReference(item));
            }

            foreach (var item in source.Usings)
            {
                builder.Usings.Add(item);
            }
        }
    }
}
