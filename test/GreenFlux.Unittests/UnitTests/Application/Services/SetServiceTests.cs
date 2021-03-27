using GreenFlux.Application.Models;
using GreenFlux.Application.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GreenFlux.Unittests.UnitTests.Application.Services
{
    public class SetServiceTests
    {
        private readonly SetService _target = new SetService();

        [Theory]
        [MemberData(nameof(TestData))]
        public void GivenAGroup_WhenGetSuggestions_ResultShouldBeExpectedSets(int sumNeeded, int[] values, int[][] expectedSets)
        {
            //act
            var result = _target
                .FindSmallestSetsForRequiredSum<IntWrapper>(values.Select(i => new IntWrapper(i)), sumNeeded, int.MaxValue, true)
                .Select(r => r.Select(v => v.Value).ToArray())
                .ToArray();

            //assert
            Assert.Equal(expectedSets, result);
        }

        public static IEnumerable<object[]> TestData =>
            new List<object[]>
            {
                new object[]
                {
                    1,
                    new []{ 1, 2, 3 },
                    new []
                    {
                        new []{ 1 }
                    }
                },
                new object[]
                {
                    3,
                    new []{ 1, 2, 3 },
                    new []{new []{ 3 }}
                },
                new object[]
                {
                    3,
                    new []{ 1, 2, 2 },
                    new []
                    {
                        new []{ 1, 2 },
                        new []{ 1, 2 }
                    }
                },
                new object[]
                {
                    4,
                    new []{ 1, 2, 2 },
                    new []
                    {
                        new []{ 2, 2 }
                    }
                },
                new object[]
                {
                    24,
                    new []{ 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23 },
                    new []
                    {
                        new []{ 1, 23 },
                        new []{ 3, 21 },
                        new []{ 5, 19 },
                        new []{ 7, 17 },
                        new []{ 9, 15 },
                        new []{ 11, 13 }
                    }
                },
                new object[]
                {
                    124,
                    new []{ 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 100 },
                    new []
                    {
                        new []{ 1, 23, 100 },
                        new []{ 3, 21, 100 },
                        new []{ 5, 19, 100 },
                        new []{ 7, 17, 100 },
                        new []{ 9, 15, 100 },
                        new []{ 11, 13, 100 }
                    }
                },
            };

        private class IntWrapper : IValue
        {
            public IntWrapper(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}
