// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Services.SingleFileApplication
{
    /// <summary>
    /// Provides a common factory to streamline the creation of the scripting types.
    /// </summary>
    public abstract class ScriptingRuntimeTypesFactory
    {
        /// <summary>
        /// Creates the scripting types.
        /// </summary>
        /// <returns></returns>
        public ScriptingRuntimeTypes CreateScriptingRuntimeTypes()
        {
            var analysisOnlyUsings = new List<string>();

            AddAnalysisOnlyUsings(analysisOnlyUsings);

            var knownTypes = new List<Type>();

            var loader = new NaiveTypeLoader();

            loader.AddKnownAssembliesByName(GetRuntimeAssemblies());

            AddKnownTypes(knownTypes);

            foreach (var item in knownTypes)
            {
                loader.AddRootType(item);
            }

            // This is to reference only the subset of things that are used for interacting with the provided root types.
            // We want to do this in order to limit showing many useless things in the suggestions (when the analysis feature is used).
            // Note that since we are running in the same AppDomain, the user will not see the suggestions for certain types,
            // but will be able to reference and use whatever we have in the AppDomain. In CoreClr there is no AppDomain management
            // therefore if you do not want this, you need to create an out of process executor.
            loader.Visit();

            var analysisAssemblies = loader.GetAssemblies();

            var analysis = new ScriptingRuntimeTypeInfo
            {
                Assemblies = analysisAssemblies,
                KnownTypes = [.. knownTypes],
                Usings = [.. analysisOnlyUsings]
            };

            var compilationKnownTypes = new List<Type>();

            AddCompilationTypeToAssembliesKnownTypes(compilationKnownTypes);

            foreach (var item in compilationKnownTypes)
            {
                loader.AddRootType(item);
            }

            // Note that we cumulate the already loaded assemblies.
            loader.Visit();

            var compilationAssemblies = loader.GetAssemblies();

            var compilation = new ScriptingRuntimeTypeInfo
            {
                Assemblies = compilationAssemblies,
                KnownTypes = [.. knownTypes],
                Usings = [],
            };

            return new ScriptingRuntimeTypes
            {
                AnalysisTypes = analysis,
                CompilationTypes = compilation,
            };
        }

        /// <summary>
        /// Adds the using clauses during for the analysis phase.
        /// </summary>
        /// <param name="list">The list of currently available using clauses.</param>
        protected abstract void AddAnalysisOnlyUsingsCore(IList<string> list);

        /// <summary>
        /// Adds the types used for inferring the assemblies to load at compile time.
        /// </summary>
        /// <param name="source"></param>
        protected abstract void AddCompilationTypeToAssembliesKnownTypesCore(IList<Type> source);

        /// <summary>
        /// Adds the known types.
        /// </summary>
        /// <param name="knownTypes"></param>
        protected abstract void AddKnownTypesCore(IList<Type> knownTypes);

        /// <summary>
        /// Gets the list of all the assemblies that represent the runtime and must be referenced.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetRuntimeAssemblies()
        {
            return
            [
                "System.Private.CoreLib",
                "System.Runtime",
                "System.Collections",
                "System.Collections.Immutable",
            ];
        }

        private void AddAnalysisOnlyUsings(List<string> list)
        {
            ArgumentNullException.ThrowIfNull(list);

            AddAnalysisOnlyUsingsCore(list);
        }

        private void AddCompilationTypeToAssembliesKnownTypes(List<Type> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            // Here we must add all the assemblies that we need in order to compile.
            // We cannot rely on any automatism and inference of the scripting engine since
            // it will use Assembly.Location for things that we do not provide explicitly
            // and this does not allow the script to run when hosted in SFAs.

            AddCompilationTypeToAssembliesKnownTypesCore(source);
        }

        /// <summary>
        /// Types (and their containing namespaces) that the user can reference without adding "usings".
        /// </summary>
        /// <param name="knownTypes"></param>
        private void AddKnownTypes(List<Type> knownTypes)
        {
            ArgumentNullException.ThrowIfNull(knownTypes);

            knownTypes.Add(typeof(Enumerable));
            knownTypes.Add(typeof(List<>));

            AddKnownTypesCore(knownTypes);
        }
    }
}
