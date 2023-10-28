// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Globalization;
using Xunit;

namespace CrudUnitTests
{
    public sealed class ObjectConversionTheoryData : TheoryData<int, object?, Type, object?>
    {
        public ObjectConversionTheoryData()
        {
            Add(01, 1, typeof(byte), (byte)1);
            Add(02, 2, typeof(short), (short)2);
            Add(03, 3, typeof(int), (int)3);
            Add(04, 4, typeof(long), (long)4);
            Add(05, -1, typeof(sbyte), (sbyte)-1);
            Add(06, -2, typeof(short), (short)-2);
            Add(07, -3, typeof(int), (int)-3);
            Add(08, -4, typeof(long), (long)-4);
            Add(09, "-4", typeof(long), (long)-4);

            Add(11, 1, typeof(byte?), (byte?)1);
            Add(12, 2, typeof(short?), (short?)2);
            Add(13, 3, typeof(int?), (int?)3);
            Add(14, 4, typeof(long?), (long?)4);
            Add(15, -1, typeof(sbyte?), (sbyte?)-1);
            Add(16, -2, typeof(short?), (short?)-2);
            Add(17, -3, typeof(int?), (int?)-3);
            Add(18, -4, typeof(long?), (long?)-4);
            Add(19, "-4", typeof(long?), (long?)-4);

            Add(21, null, typeof(byte?), (byte?)null);
            Add(22, null, typeof(short?), (short?)null);
            Add(23, null, typeof(int?), (int?)null);
            Add(24, null, typeof(long?), (long?)null);
            Add(25, null, typeof(sbyte?), (sbyte?)null);
            Add(26, null, typeof(short?), (short?)null);
            Add(27, null, typeof(int?), (int?)null);
            Add(28, null, typeof(long?), (long?)null);
            Add(29, "", typeof(long?), (long?)null);

            Add(30, null, typeof(string), (string?)null);
            Add(31, "", typeof(string), (string?)string.Empty);
            Add(32, "aa", typeof(string), (string)"aa");
            Add(33, 1.4f, typeof(float), (float)1.4);
            Add(34, 1.4f, typeof(float?), (float?)1.4);
            Add(35, null, typeof(float?), (float?)null);
            Add(36, "1.4", typeof(float?), (float?)1.4);
            Add(37, "", typeof(float?), (float?)null);
            Add(38, "1.4", typeof(float), (float)1.4);

            Add(43, 1.4d, typeof(double), (double)1.4);
            Add(44, 1.4d, typeof(double?), (double?)1.4);
            Add(45, null, typeof(double?), (double?)null);
            Add(46, "1.4", typeof(double?), (double?)1.4);
            Add(47, "", typeof(double?), (double?)null);
            Add(48, "1.4", typeof(double), (double)1.4);

            var dt = WipeMilliseconds(DateTime.UtcNow);
            Add(50, dt, typeof(DateTime), (DateTime)dt);
            Add(51, dt, typeof(DateTime?), (DateTime?)dt);
            Add(52, null, typeof(DateTime?), (DateTime?)null);
            Add(53, dt.ToIso8601String(), typeof(DateTime), (DateTime)dt);
            Add(54, dt.ToIso8601String(), typeof(DateTime?), (DateTime?)dt);

            var dto = WipeMilliseconds(DateTimeOffset.UtcNow);
            Add(60, dto, typeof(DateTimeOffset), (DateTimeOffset)dto);
            Add(61, dto, typeof(DateTimeOffset?), (DateTimeOffset?)dto);
            Add(62, null, typeof(DateTimeOffset?), (DateTimeOffset?)null);
            Add(63, dto.ToString(CultureInfo.InvariantCulture), typeof(DateTimeOffset), (DateTimeOffset)dto);
            Add(64, dto.ToString(CultureInfo.InvariantCulture), typeof(DateTimeOffset?), (DateTimeOffset?)dto);

            var guid = Guid.NewGuid();
            Add(70, guid, typeof(Guid), (Guid)guid);
            Add(71, guid, typeof(Guid?), (Guid?)guid);
            Add(72, null, typeof(Guid?), (Guid?)null);
            Add(73, guid.ToString(), typeof(Guid), (Guid)guid);
            Add(74, guid.ToString(), typeof(Guid?), (Guid?)guid);
            Add(76, "", typeof(Guid), (Guid)Guid.Empty);
            Add(77, "", typeof(Guid?), (Guid?)null);

            Add(80, "", typeof(MyEnumType?), (MyEnumType?)null);
            Add(81, "15", typeof(MyEnumType), (MyEnumType)MyEnumType.Value1);
            Add(82, "15", typeof(MyEnumType?), (MyEnumType?)MyEnumType.Value1);
            //Add(83, "value1", typeof(MyEnum), (MyEnum)MyEnum.Value1);
            //Add(84, "value1", typeof(MyEnum?), (MyEnum?)MyEnum.Value1);
        }

        private static DateTimeOffset WipeMilliseconds(DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Offset);
        }

        private static DateTime WipeMilliseconds(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, DateTimeKind.Utc);
        }
    }
}
