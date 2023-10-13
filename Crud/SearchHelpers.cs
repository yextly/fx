// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc.Crud
{
    internal sealed class SearchHelpers
    {
        public static bool Like(string matchExpression, string pattern)
        {
            if (string.IsNullOrEmpty(matchExpression))
                return false;

            if (string.IsNullOrEmpty(pattern))
                return true;

            return matchExpression.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}
