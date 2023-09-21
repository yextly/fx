// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    internal static class ReflectionHelpers
    {
        public static bool EqualsInternal<T>(T left, T right) where T : struct, IEquatable<T>
        {
            return left.Equals((T)right);
        }

        public static bool CanBeAssignedToRawGeneric(Type sourceType, Type destinationType)
        {
            while (sourceType != null && sourceType != typeof(object))
            {
                var cur = sourceType.IsGenericType ? sourceType.GetGenericTypeDefinition() : sourceType;
                if (destinationType == cur)
                {
                    return true;
                }
                if (sourceType.BaseType == null)
                    return false;

                sourceType = sourceType.BaseType;
            }
            return false;
        }

        public static bool TryGetDbContextType(Type type, [MaybeNullWhen(false)] out Type innerType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(DbSet<>)))
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

        public static PropertyInfo GetPropertyByName<T>(string name)
        {
            return typeof(T).GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase) ?? throw new InvalidOperationException($@"The property ""{name}"" is not found.");
        }

        public static MethodInfo GetEFLikeMethod()
        {
            return typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(DbFunctions), typeof(string), typeof(string) }) ?? throw new InvalidOperationException("Cannot find EF.Functions.Like.");
        }

        public static MethodInfo GetSystemStringCompareMethod()
        {
            return typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string), typeof(StringComparison) }) ?? throw new InvalidOperationException("Cannot find System.String.Equals.");
        }
    }
}
