// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Globalization;

namespace System
{
    /// <summary>
    /// Provides extensions for <see cref="long"/>.
    /// </summary>
    public static class Int64Extensions
    {
        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns></returns>
        public static string ToStringInvariant(this long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
