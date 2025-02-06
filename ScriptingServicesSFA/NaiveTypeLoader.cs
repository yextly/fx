// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;
using System.Reflection;

namespace Yextly.Scripting.Services.SingleFileApplication
{
    /// <summary>
    /// Poor man reachability with side effects (the runtime loads the assemblies for us in the proper way).
    /// </summary>
    internal sealed class NaiveTypeLoader
    {
        private readonly HashSet<Assembly> _assemblies = [];
        private readonly Queue<Type> _types = new();
        private readonly HashSet<Type> _visitedTypes = [];

        public NaiveTypeLoader()
        {
        }

        public void AddKnownAssemblies(IEnumerable<Assembly> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            _assemblies.AddRange(source);
        }

        public void AddKnownAssembliesByName(IEnumerable<string> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            _assemblies.AddRange(GetAssembliesFromString(source));
        }

        public void AddRootType(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            Enqueue(type);
        }

        public ImmutableArray<Assembly> GetAssemblies()
        {
            // We sort for reproducibility purposes
            return
            [
                .. _assemblies
                    .OrderBy(x => x.FullName, StringComparer.Ordinal)
            ];
        }

        public void Visit()
        {
            while (_types.TryDequeue(out var type))
            {
                foreach (var item in type.GetRuntimeProperties())
                {
                    Enqueue(item.PropertyType);
                }

                foreach (var item in type.GetRuntimeMethods())
                {
                    Enqueue(item.ReturnType);

                    foreach (var p in item.GetParameters())
                    {
                        Enqueue(p.ParameterType);
                    }
                }

                foreach (var item in type.GetInterfaces())
                {
                    Enqueue(item);
                }

                if (type.BaseType != null)
                    Enqueue(type.BaseType);

                if (type.IsGenericTypeParameter)
                {
                    foreach (var item in type.GenericTypeArguments)
                    {
                        Enqueue(item);
                    }
                }
            }
        }

        private static IEnumerable<Assembly> GetAssembliesFromString(IEnumerable<string> source)
        {
            var allAssemblies = new Dictionary<string, Assembly>(StringComparer.Ordinal);

            foreach (var item in AppDomain.CurrentDomain.GetAssemblies().AsEnumerable())
            {
                var name = item.GetName().Name;
                if (name != null)
                    allAssemblies.TryAdd(name, item);
            }

            // https://stackoverflow.com/questions/23907305/roslyn-has-no-reference-to-system-runtime
            foreach (var item in source)
            {
                if (allAssemblies.TryGetValue(item, out var assembly))
                {
                    yield return assembly;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to find the assembly {item}.");
                }
            }
        }

        private void Enqueue(Type type)
        {
            _assemblies.Add(type.Assembly);

            if (_visitedTypes.Add(type))
            {
                _types.Enqueue(type);
            }
        }
    }
}
