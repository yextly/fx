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
    public sealed class DeleteTests
    {
        [Fact]
        public async Task CanDeleteASingleEntry()
        {
            var data = Data1Storage.CreateList();
            var originalData = Data1Storage.CreateList();
            var adapter = new Mock<ICrudDataAdapter<Data1>>();
            var logger = ProxyUtilities.CreateControllerLogger();
            var source = data.AsQueryable();

            adapter
                .Setup(x => x.PrepareForWriting(source))
                .Returns(source);
            adapter
                .Setup(x => x.AllowDeleteOperations)
                .Returns(true);
            adapter.Setup(x => x.PrepareForReading(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();
            adapter.Setup(x => x.Delete(It.IsAny<Data1>()))
                .Callback<Data1>(x => data.Remove(x));

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new DeleteEntityRequest<Data1>
            {
                Id = "404",
                PreviousData = null,
            };

            var result = await controller.DeleteEntityForTesting(request).ConfigureAwait(true);

            Assert.NotNull(result);

            adapter.Verify(x => x.PrepareForWriting(source));

            var expected = originalData.Single(x => x.Id == 404);
            var actual = result;

            Assert.Equal(expected, actual);

            originalData.RemoveAt(originalData.FindIndex(x => x.Id == 404));

            Assert.Equal(originalData, data);
        }
    }
}
