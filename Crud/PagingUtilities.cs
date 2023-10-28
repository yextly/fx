// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc.Crud
{
    internal static class PagingUtilities
    {
        private const int DefaultPageSize = 10;
        private const int MaximumPageSize = 1000;

        public static IQueryable<T> Page<T>(IQueryable<T> source, int first, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (first < 0)
                first = 0;
            if (count <= 0)
                count = DefaultPageSize;

            if (count > MaximumPageSize)
                count = MaximumPageSize;

            return source.Skip(first).Take(count);
        }
    }
}
