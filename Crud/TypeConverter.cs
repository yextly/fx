// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Globalization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    internal static class TypeConverter
    {
        private static object? ChangeTypeAndLift(object? value, Type newType, object? defaultValue)
        {
            if (newType == typeof(object))
            {
                // this is the quick path for non-IConvertible instances
                return (object?)value;
            }
            else if (newType.IsEnum && value != null)
            {
                if (value is string s)
                    return Enum.ToObject(newType, Int64.Parse(s, CultureInfo.InvariantCulture));
                else
                    return Enum.ToObject(newType, value);
            }
            else if (newType == typeof(DateTimeOffset) && value is string dtos)
            {
                return DateTimeOffset.Parse(dtos, CultureInfo.InvariantCulture);
            }
            else if (newType == typeof(DateTimeOffset?) && value is string ndtos)
            {
                return DateTimeOffset.Parse(ndtos, CultureInfo.InvariantCulture);
            }
            else if (newType == typeof(Guid) && value is string gs)
            {
                if (gs.Length == 0)
                    return Guid.Empty;
                else
                    return Guid.Parse(gs);
            }
            else if (newType == typeof(Guid?) && value is string ngs)
            {
                if (ngs.Length == 0)
                    return null;
                else
                    return Guid.Parse(ngs);
            }
            else
            {
                if (value is string s && s.Length == 0 && newType != typeof(string))
                {
                    // if we are NOT converting to a string and object, and the value is currently an empty string,
                    // it is probable that the chosen type converter won't be able to handle the empty string.
                    // for this reason, we turn it a null, which should be more manageable.
                    value = null;
                }

                var nt = Nullable.GetUnderlyingType(newType);
                if (nt == null)
                {
                    // we can go straight to the conversion path as we have no other monads to handle.
                    return Convert.ChangeType(value, newType, CultureInfo.InvariantCulture);
                }
                else
                {
                    if (value == null)
                    {
                        // here we would require default(T) which can be obtained in few ways via reflection. As this method should not be generic, we kindly
                        // ask the user to give us the default value as this method is always called in a generic context.
                        return defaultValue;
                    }
                    else if (nt.IsEnum)
                    {
                        if (value is long d)
                        {
                            // here the conversion is to the maximum storage type allowed for enums
                            return Enum.ToObject(nt, (long)d);
                        }
                        else if (value is string nnss)
                        {
                            return Enum.ToObject(nt, Int64.Parse(nnss, CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            return Enum.ToObject(nt, value);
                        }
                    }
                    else
                    {
                        // warning:
                        // this is the intermediate cast as asked by the user. This means that we are yielding non nullable types here on purpose.
                        // in order to complete the operation you must cast this value to the correct nullable type. We are not doing that here as the caller
                        // is already doing this cast: here we would use the reflection to implement that cast which would be totally useless when as the caller
                        // would redo the same job in a more efficient way.
                        return Convert.ChangeType(value, nt, CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        public static object? ConvertFromObject(object? value, Type newType)
        {
            return ChangeTypeAndLift(value, newType, null);
        }
    }
}
