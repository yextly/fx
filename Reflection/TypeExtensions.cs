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
        /// Checks whether <paramref name="type"/> is closed generic type definition and returns the first innner type definition.
        /// </summary>
        /// <param name="type">The type being tested.</param>
        /// <param name="expectedTypeDefinition">The open generic type definition to match.</param>
        /// <param name="innerType">The inner type extracted definition. In case of multiple types, the first one is returned.</param>
        /// <returns></returns>
        public static bool IsGenericTypeOfT(this Type type, Type expectedTypeDefinition, [MaybeNullWhen(false)] out Type innerType)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(expectedTypeDefinition);

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == expectedTypeDefinition))
            {
                innerType = type.GetGenericArguments()[0];
                return true;
            }
            else
            {
                innerType = default!;
                return false;
            }
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> implements a generic interface on the form IInterface of T.
        /// </summary>
        /// <param name="type">The type being tested.</param>
        /// <param name="expectedTypeDefinition">The open generic type definition to match.</param>
        /// <param name="innerType">The inner type extracted by the <see cref="List{T}"/> definition.</param>
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
        /// Checks whether <paramref name="type"/> is on the form of <see cref="Task{TResult}"/> and returns is innner type definition.
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
