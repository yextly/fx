// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Provides extensions for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks whether <paramref name="type"/> implements a generic interface on the form IInterface of T.
        /// </summary>
        /// <param name="type">The type being tested.</param>
        /// <param name="expectedTypeDefinition">The open generic type definition to match.</param>
        /// <param name="innerType">The first inner type extracted by the generic definition.</param>
        /// <returns></returns>
        public static bool ImplementsGenericInterfaceOfT(this Type type, Type expectedTypeDefinition, [MaybeNullWhen(false)] out Type innerType)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(expectedTypeDefinition);

            if (!expectedTypeDefinition.IsInterface)
                throw new ArgumentException("The provided type is not an interface", nameof(expectedTypeDefinition));

            if (IsGenericTypeOfT(type, expectedTypeDefinition, out var r))
            {
                innerType = r;
                return true;
            }
            var interfaces = type.GetInterfaces();

            foreach (var item in interfaces)
            {
                if (IsGenericTypeOfT(item, expectedTypeDefinition, out var ret))
                {
                    innerType = ret;
                    return true;
                }
            }

            innerType = null;
            return false;
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> implements a generic interface on the form IInterface of T.
        /// </summary>
        /// <param name="type">The type being tested.</param>
        /// <param name="expectedTypeDefinition">The open generic type definition to match.</param>
        /// <param name="innerTypes">The inner types extracted by the generic definition.</param>
        /// <returns></returns>
        public static bool ImplementsGenericInterfaceOfTWithArray(this Type type, Type expectedTypeDefinition, [MaybeNullWhen(false)] out Type[] innerTypes)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(expectedTypeDefinition);

            if (!expectedTypeDefinition.IsInterface)
                throw new ArgumentException("The provided type is not an interface", nameof(expectedTypeDefinition));

            if (IsGenericTypeOfTWithArray(type, expectedTypeDefinition, out var r))
            {
                innerTypes = r;
                return true;
            }
            var interfaces = type.GetInterfaces();

            foreach (var item in interfaces)
            {
                if (IsGenericTypeOfTWithArray(item, expectedTypeDefinition, out var ret))
                {
                    innerTypes = ret;
                    return true;
                }
            }

            innerTypes = null;
            return false;
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> is a closed generic type definition and returns the first inner type definition.
        /// </summary>
        /// <param name="type">The type being tested.</param>
        /// <param name="expectedTypeDefinition">The open generic type definition to match.</param>
        /// <param name="innerType">The inner type extracted definition. In case of multiple types, the first one is returned.</param>
        /// <returns></returns>
        public static bool IsGenericTypeOfT(this Type type, Type expectedTypeDefinition, [MaybeNullWhen(false)] out Type innerType)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(expectedTypeDefinition);

            if (IsGenericTypeOfTWithArray(type, expectedTypeDefinition, out var types))
            {
                innerType = types[0];
                return true;
            }
            else
            {
                innerType = default!;
                return false;
            }
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> is a closed generic type definition and returns the inner type array definition.
        /// </summary>
        /// <param name="type">The type being tested.</param>
        /// <param name="expectedTypeDefinition">The open generic type definition to match.</param>
        /// <param name="innerTypes">The list of extracted definition types.</param>
        /// <returns></returns>
        public static bool IsGenericTypeOfTWithArray(this Type type, Type expectedTypeDefinition, [MaybeNullWhen(false)] out Type[] innerTypes)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(expectedTypeDefinition);

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == expectedTypeDefinition))
            {
                innerTypes = type.GetGenericArguments();
                return true;
            }
            else
            {
                innerTypes = default!;
                return false;
            }
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> is on the form of <see cref="Task{TResult}"/> and returns is inner type definition.
        /// </summary>
        /// <param name="type">The type being tested.</param>
        /// <param name="innerType">The inner type extracted by the <see cref="Task{TResult}"/> definition.</param>
        /// <returns></returns>
        public static bool IsTaskOfT(this Type type, [MaybeNullWhen(false)] out Type innerType)
        {
            ArgumentNullException.ThrowIfNull(type);

            Type? t = type;

            while (t != null)
            {
                if (IsGenericTypeOfT(t, typeof(Task<>), out innerType))
                    return true;
                else
                    t = t.BaseType;
            }

            innerType = null;
            return false;
        }
    }
}
