// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Yextly.ServiceFabric.Mvc.Crud;

namespace CrudUnitTests
{
    public sealed class ReadTests
    {
        [ClassData(typeof(AllCharactersTheoryData))]
        [Theory]
        public async Task CanReadById(int id)
        {
            var data = Data1Storage.CreateList();
            var adapter = new Mock<ICrudDataAdapter<Data1>>();
            var logger = ProxyUtilities.CreateControllerLogger();
            var source = data.AsQueryable();
            adapter
                .Setup(x => x.PrepareForReading(source))
                .Returns(source);
            adapter.Setup(x => x.PrepareForWriting(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new GetMultipleEntitiesRequest
            {
                Ids = new string[] { id.ToStringInvariant() },
            };

            var result = await controller.GetMultipleForTesting(request).ConfigureAwait(true);

            adapter.Verify(x => x.PrepareForReading(source));
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(-1, result.Count);
            Assert.Single(result.Data);

            var expected = data.Single(x => x.Id == id);
            var actual = result.Data[0];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CanGetMultipleRecords()
        {
            var data = Data1Storage.CreateList();
            var adapter = new Mock<ICrudDataAdapter<Data1>>();
            var logger = ProxyUtilities.CreateControllerLogger();
            var source = data.AsQueryable();
            adapter
                .Setup(x => x.PrepareForReading(source))
                .Returns(source);
            adapter.Setup(x => x.PrepareForWriting(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();

            var expectedItems = new int[] { 111, 222, 333, 444, 555, 666, 777, 888 };

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new GetMultipleEntitiesRequest
            {
                Ids = expectedItems.Select(x => x.ToStringInvariant()).ToArray(),
            };

            var result = await controller.GetMultipleForTesting(request).ConfigureAwait(true);

            adapter.Verify(x => x.PrepareForReading(source));
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(-1, result.Count);
            Assert.Equal(expectedItems.Length, result.Data.Count);

            var expected = data.Where(x => expectedItems.Contains(x.Id)).OrderBy(x => x.Id).ToList();
            var actual = result.Data.OrderBy(x => x.Id).ToList();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CanReadFilteredAndSortedData()
        {
            var data = Data1Storage.CreateList();
            var adapter = new Mock<ICrudDataAdapter<Data1>>();
            var logger = ProxyUtilities.CreateControllerLogger();
            var source = data.AsQueryable();
            adapter
                .Setup(x => x.PrepareForReading(source))
                .Returns(source);
            adapter.Setup(x => x.PrepareForWriting(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new GetListRequest
            {
                Filter = new FilterFieldInfo[]
                {
                     new FilterFieldInfo
                     {
                          Name = "allegiances", Value = "st",
                     },
                     new FilterFieldInfo
                     {
                          Name = "introduction", Value = "16",
                     },
                },
                Sorting = new SortFieldInfo[]
                 {
                      new SortFieldInfo { Name =   "id", SortType = SortType.Descending
                      },
                 },
                Range = new RangeInfo
                {
                    Start = 1,
                    End = 3,
                }
            };

            var result = await controller.GetListForTesting(request).ConfigureAwait(true);

            adapter.Verify(x => x.PrepareForReading(source));
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            var expectedItems = new int[] { 696, 654, 636 };

            Assert.Equal(12, result.Count);
            Assert.Equal(expectedItems.Length, result.Data.Count);

            var expected = expectedItems.Select(x => data.Single(y => y.Id == x)).ToArray();

            var actual = result.Data;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CanReadFilteredAndSortedDataOnGuid()
        {
            var data = Data1Storage.CreateList();
            var adapter = new Mock<ICrudDataAdapter<Data1>>();
            var logger = ProxyUtilities.CreateControllerLogger();
            var source = data.AsQueryable();
            adapter
                .Setup(x => x.PrepareForReading(source))
                .Returns(source);
            adapter.Setup(x => x.PrepareForWriting(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new GetListRequest
            {
                Filter = new FilterFieldInfo[]
                {
                     new FilterFieldInfo
                     {
                          Name = "IdAsGuid", Value = "00000337-0000-0000-0000-000000000000",
                     },
                },
                Sorting = new SortFieldInfo[]
                {
                     new SortFieldInfo
                     {
                         Name =   "id", SortType = SortType.Descending
                     },
                }
            };

            var result = await controller.GetListForTesting(request).ConfigureAwait(true);

            adapter.Verify(x => x.PrepareForReading(source));
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            var expectedItems = new int[] { 823 };

            Assert.Equal(1, result.Count);
            Assert.Equal(expectedItems.Length, result.Data.Count);

            var expected = expectedItems.Select(x => data.Single(y => y.Id == x)).ToArray();

            var actual = result.Data;

            Assert.Equal(expected, actual);
        }
    }
}
