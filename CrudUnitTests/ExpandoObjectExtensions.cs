// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace CrudUnitTests
{
    internal static class ExpandoObjectExtensions
    {
        public static void AddProperty<T, Q>(this ExpandoObject instance, Expression<Func<T, Q>> expression, Q value)
        {
            var dictionary = (IDictionary<string, object?>)instance;
            var property = GetPropertyInfo2(expression) ?? throw new InvalidOperationException("Cannot find the property.");
            dictionary.Add(property.Name, value);
        }

        private static PropertyInfo? GetPropertyInfo2<T, Q>(Expression<Func<T, Q>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            // here we accept x.Property1, no nesting

            if (expression.Body is not MemberExpression body)
            {
                UnaryExpression ubody = (UnaryExpression)expression.Body;
                if (ubody.Operand is not MemberExpression t)
                    return null;

                body = t;
            }

            return body.Member as PropertyInfo;
        }
    }
}
