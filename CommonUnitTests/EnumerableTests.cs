// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CommonUnitTests
{
    public sealed class EnumerableTests
    {
        [Fact]
        public void CanRepeatAnEmptySequence()
        {
            var input = Enumerable.Empty<int>();

            var expected = Enumerable.Empty<int>();
            var actual = input.RepeatAfter(x => x == 10);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWhenThePredicateDoesNotFire()
        {
            var input = Enumerable.Range(0, 12);

            var expected = Enumerable.Range(0, 12);
            var actual = input.RepeatAfter(x => x == 20);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWhenThePredicateFires()
        {
            var input = Enumerable.Range(0, 12);

            var expected = new int[] { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2 };
            var actual = input.RepeatAfter(x => x == 3);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWhenThePredicateFiresAtTheEnd()
        {
            var input = Enumerable.Range(0, 12);

            var expected = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0 };
            var actual = input.RepeatAfter(x => x == 11);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWhenThePredicateFiresPastTheEnd()
        {
            var input = Enumerable.Range(0, 12);

            var expected = Enumerable.Range(0, 12);
            var actual = input.RepeatAfter(x => x == 12);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWithOneElement()
        {
            var input = Enumerable.Range(0, 1);

            var expected = Enumerable.Range(0, 1);
            var actual = input.RepeatAfter(x => x == 11);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWithOneElementAndThePredicateFiring()
        {
            var input = Enumerable.Range(0, 1);

            var expected = Enumerable.Range(0, 1);
            var actual = input.RepeatAfter(x => x == 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWithOneElementAndThePredicateFiringLater()
        {
            var input = Enumerable.Range(0, 1);

            var expected = Enumerable.Range(0, 1);
            var actual = input.RepeatAfter(x => x == 1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWithTwoElements()
        {
            var input = Enumerable.Range(0, 2);

            var expected = Enumerable.Range(0, 2);
            var actual = input.RepeatAfter(x => x == 11);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWithTwoElementsAndThePredicateFiringLater()
        {
            var input = Enumerable.Range(0, 2);

            var expected = new int[] { 0, 1 };
            var actual = input.RepeatAfter(x => x == 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWithTwoElementsAndThePredicateFiringOnTheFirst()
        {
            var input = Enumerable.Range(0, 2);

            var expected = new int[] { 0, 1 };
            var actual = input.RepeatAfter(x => x == 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanRepeatWithTwoElementsAndThePredicateFiringOnTheSecond()
        {
            var input = Enumerable.Range(0, 2);

            var expected = new int[] { 0, 0 };
            var actual = input.RepeatAfter(x => x == 1);

            Assert.Equal(expected, actual);
        }
    }
}
