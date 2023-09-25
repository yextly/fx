// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ReflectionUnitTests
{
    public sealed class ReflectionTests
    {
        [InlineData(typeof(Task<int>), typeof(Task<>), typeof(int))]
        [InlineData(typeof(List<int>), typeof(List<>), typeof(int))]
        [InlineData(typeof(List<string>), typeof(List<>), typeof(string))]
        [Theory]
        public void CanExtractGenericTypeOfT(Type input, Type definition, Type expected)
        {
            var success = input.IsGenericTypeOfT(definition, out var actual);

            Assert.True(success);
            Assert.Equal(expected, actual);
        }

        [InlineData(typeof(IReadOnlyList<int?>), typeof(IReadOnlyList<>), typeof(int?))]
        [InlineData(typeof(List<int?>), typeof(IReadOnlyList<>), typeof(int?))]
        [InlineData(typeof(List<string>), typeof(IReadOnlyList<>), typeof(string))]
        [Theory]
        public void CanExtractInterfaceOfT(Type input, Type definition, Type expected)
        {
            var success = input.ImplementsGenericInterfaceOfT(definition, out var actual);

            Assert.True(success);
            Assert.Equal(expected, actual);
        }

        [InlineData(typeof(IReadOnlyDictionary<int, long?>), typeof(IReadOnlyDictionary<,>), typeof(int), typeof(long?))]
        [InlineData(typeof(Dictionary<int, long?>), typeof(IReadOnlyDictionary<,>), typeof(int), typeof(long?))]
        [Theory]
        public void CanExtractInterfaceOfTArray(Type input, Type definition, params Type[] expected)
        {
            var success = input.ImplementsGenericInterfaceOfTWithArray(definition, out var actual);

            Assert.True(success);
            Assert.Equal(expected, actual);
        }

        [InlineData(typeof(Task<int>), typeof(int))]
        [InlineData(typeof(Task<int?>), typeof(int?))]
        [InlineData(typeof(Task<Stream>), typeof(Stream))]
        [InlineData(typeof(Task<string>), typeof(string))]
        [InlineData(typeof(DerivedTask<string>), typeof(string))]
        [Theory]
        public void CanExtractTaskOfT(Type input, Type expected)
        {
            var success = input.IsTaskOfT(out var actual);

            Assert.True(success);
            Assert.Equal(expected, actual);
        }

        [InlineData(typeof(DerivedTask<int>), typeof(Task<>))]
        [InlineData(typeof(object), typeof(List<>))]
        [InlineData(typeof(IReadOnlyList<string>), typeof(List<>))]
        [Theory]
        public void CannotExtractGenericTypeOfT(Type input, Type definition)
        {
            var success = input.IsGenericTypeOfT(definition, out _);

            Assert.False(success);
        }

        [InlineData(typeof(object), typeof(IReadOnlyList<>))]
        [InlineData(typeof(Stream), typeof(IReadOnlyList<>))]
        [Theory]
        public void CannotExtractInterfaceOfT(Type input, Type definition)
        {
            var success = input.ImplementsGenericInterfaceOfT(definition, out _);

            Assert.False(success);
        }

        [InlineData(typeof(ValueTask<int>))]
        [InlineData(typeof(Stream))]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        [Theory]
        public void CannotExtractTaskOfT(Type input)
        {
            var success = input.IsTaskOfT(out _);

            Assert.False(success);
        }

        [Fact]
        public void NonInterfaceTypeThrows()
        {
            Assert.ThrowsAny<ArgumentException>(() => typeof(Task<int>).ImplementsGenericInterfaceOfT(typeof(int), out _));
        }
    }
}
