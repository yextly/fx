// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Common
{
    internal static class CollectionUtilities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We guarantee disposal, therefore we cannot rethrow for each item in the list (and aggregating would be a performance hit)")]
        public static void DisposeInternal<T>(IEnumerable<T> source, bool useSnapshot, Action cleanUp) where T : IDisposable
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(cleanUp);

            // We must use a snapshot when there is the chance that the collection gets mutated while disposing.
            // If this happens the enumerator throws and we fail the disposal.
            // Also note, that a failure in creating the snapshot is rethrown on purpose.
            var snapshot = (useSnapshot) ? source : source.ToList();

            try
            {
                foreach (var item in snapshot)
                {
                    try
                    {
                        using (item)
                        {
                        }
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                cleanUp();
            }
        }
    }
}
