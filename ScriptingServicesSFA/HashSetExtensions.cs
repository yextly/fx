// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Services.SingleFileApplication
{
    internal static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> instance, IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(collection);

            foreach (var item in collection)
            {
                instance.Add(item);
            }
        }
    }
}
