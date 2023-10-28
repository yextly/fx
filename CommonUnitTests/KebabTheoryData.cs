// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace CommonUnitTests
{
    /// <summary>
    /// This theory is necessary, since VSTest gets broken with certain characters when specified via InlineData.
    /// </summary>
    public sealed class KebabTheoryData : TheoryData<string?, string?>
    {
        public KebabTheoryData()
        {
            Add("abcdefghijklmnopqrstuvwxyz-0123456789", "abcdefghijklmnopqrstuvwxyz0123456789");
            Add("", "");
            Add("", "!");
            Add("", @"!""");
            Add("", @"!""£");
            Add("", @"!""£$");
            Add("", @"!""£$%");
            Add("", @"!""£$%&");
            Add("", @"!""£$%&/");
            Add("", @"!""£$%&/(");
            Add("", @"!""£$%&/()");
            Add("", @"!""£$%&/()=");
            Add("", @"!""£$%&/()=?");
            Add("", @"!""£$%&/()=?^[");
            Add("", @"!""£$%&/()=?^[]");
            Add("", @"!""£$%&/()=?^[]§");
            Add("", @"!""£$%&/()=?^[]§°");
            Add("", @"!""£$%&/()=?^[]§°#");
            Add("", @"!""£$%&/()=?^[]§°#@");
            Add("", @"!""£$%&/()=?^[]§°#@.");
            Add("", @"!""£$%&/()=?^[]§°#@.,");
            Add("", @"!""£$%&/()=?^[]§°#@.,-");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>+");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>+-");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>+-*");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>+-*/");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>+-*/.");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>+-*/.\");
            Add("", @"!""£$%&/()=?^[]§°#@.,-_<>+-*/.\|");
            Add("a", "#a");
            Add("1", "1");
            Add("", "-");
            Add("", "-------");
            Add("a-bc-d-ef", "aBc-dEf");
            Add("abc-def", "abc-def");
            Add("10-aaa", "10AAA");
            Add("10-aaa", "10aaa");
            Add("k-pi", "K-pi");
            Add("k-pi", "k-pi");
            Add("kpi", "Kpi");
            Add("kpi", "KPI");
            Add("a-a", "---a---A");
            Add("a", "-a-");
            Add(null, null);
        }
    }
}
