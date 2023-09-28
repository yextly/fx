// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System
{
    /// <summary>
    /// Provides extension to <see cref="String"/>.
    /// </summary>
    public static class StringExtensions
    {
        private enum KebabState
        {
            None = 0,
            Dash = 1,
            LowerCaseLetter = 2,
            Digit = 3,
            UpperCaseLetter = 4,
        }

        /// <summary>
        /// Converts the provided string into kebab case.
        /// </summary>
        /// <param name="source">The string to convert.</param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(source))]
        public static string? ToKebab(this string source)
        {
            if (source == null)
                return null;

            if (source.Length == 0)
                return string.Empty;

            var buffer = ArrayPool<char>.Shared.Rent(source.Length * 2);

            var nextIndex = 0;

            var state = KebabState.None;
            try
            {
                for (var i = 0; i < source.Length; i++)
                {
                    var currentCategory = char.GetUnicodeCategory(source[i]);

                    KebabState newState;
                    if (IsToBeDiscarded(currentCategory))
                    {
                        // symbols are discarded, but introduce a pending dash
                        newState = KebabState.None;
                    }
                    else
                    {
                        newState = GetState(source[i]);
                    }

                    if (newState != KebabState.None)
                    {
                        var nextChar = char.ToLowerInvariant(source[i]);

                        if (state == newState || (state == KebabState.UpperCaseLetter && newState == KebabState.LowerCaseLetter))
                        {
                            buffer[nextIndex++] = nextChar;
                        }
                        else
                        {
                            if (nextIndex != 0)
                                buffer[nextIndex++] = '-';
                            buffer[nextIndex++] = nextChar;
                        }
                    }

                    state = newState;
                }

                return new string(buffer, 0, nextIndex);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }

        private static KebabState GetState(char @char)
        {
            if (char.IsLower(@char))
                return KebabState.LowerCaseLetter;
            else if (char.IsUpper(@char))
                return KebabState.UpperCaseLetter;
            else if (char.IsDigit(@char))
                return KebabState.Digit;
            //else if (@char == '-')
            //    return KebabState.Dash;
            else
                return KebabState.None;
        }

        private static bool IsToBeDiscarded(UnicodeCategory category)
        {
            return category switch
            {
                UnicodeCategory.UppercaseLetter or UnicodeCategory.LowercaseLetter or UnicodeCategory.TitlecaseLetter or UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter or UnicodeCategory.DecimalDigitNumber or UnicodeCategory.LetterNumber => false,
                _ => true,
            };
        }
    }
}
