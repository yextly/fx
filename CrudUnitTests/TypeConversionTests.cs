// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using Xunit;
using Yextly.ServiceFabric.Mvc.Crud;

namespace CrudUnitTests
{
    public sealed class TypeConversionTests
    {
        [ClassData(typeof(ObjectConversionTheoryData))]
        [Theory]
        public void CanConvertFromObject(int id, object input, Type wantedType, object? expected)
        {
            Assert.True(id > 0);

            var actual = TypeConverter.ConvertFromObject(input, wantedType);

            Assert.Equal(expected, actual);
        }
    }
}
