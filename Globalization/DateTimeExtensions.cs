// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace System
{
    /// <summary>
    /// Provides extension to <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts the value of the specified <paramref name="value"/> object to its equivalent string representation using the ISO format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns></returns>
        public static string ToIso8601String(this DateTime value)
        {
            return value.ToString("O", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
