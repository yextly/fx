// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Moq;
using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Yextly.ServiceFabric.Mvc.Crud;

namespace CrudUnitTests
{
    public sealed class UpdateTests
    {
        [Fact]
        public async Task CanUpdateMultipleEntriesWithPatch()
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
                .Setup(x => x.AllowUpdateOperations)
                .Returns(true);
            adapter.Setup(x => x.PrepareForReading(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();
            adapter
                .Setup(x => x.UpdateEntity(It.IsAny<Data1>()))
                .Returns<Data1>(x => x);

            var controller = new Data1Controller(logger, adapter.Object, source);

            var idsToUpdate = new int[] { 23, 456, 314, 2, 8 };

            var request = new UpdateMultipleEntitiesRequest<Data1>
            {
                Ids = idsToUpdate.Select(x => x.ToStringInvariant()).ToArray(),
                Data = new ExpandoObject(),
            };

            request.Data.AddProperty<Data1, string>(x => x.Allegiances, "Unknown");

            var result = await controller.UpdateEntitiesForTesting(request).ConfigureAwait(true);

            Assert.NotNull(result);

            adapter.Verify(x => x.PrepareForWriting(source));

            var expected = idsToUpdate.Select(x => x.ToStringInvariant()).OrderBy(x => x).ToArray();
            var actual = result.Ids.OrderBy(x => x).ToArray();

            Assert.Equal(expected, actual);

            foreach (var item in idsToUpdate)
            {
                originalData[originalData.FindIndex(x => x.Id == item)].Allegiances = "Unknown";
            }

            Assert.Equal(originalData, data);
        }

        [Fact]
        public async Task CanUpdateSingleEntryWithPatch()
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
                .Setup(x => x.AllowUpdateOperations)
                .Returns(true);
            adapter.Setup(x => x.PrepareForReading(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();
            adapter
                .Setup(x => x.UpdateEntity(It.IsAny<Data1>()))
                .Returns<Data1>(x => x);

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new UpdateSingleEntityRequest<Data1>
            {
                Id = "402",
                PreviousData = new ExpandoObject(),
                Data = new ExpandoObject(),
            };

            var oldEntity = data.Single(x => x.Id == 402);
            var newEntity = oldEntity with
            {
                Name = "Aegon Targaryen",
                Introduction = 63,
            };

            request.PreviousData.AddProperty<Data1, string>(x => x.Name, oldEntity.Name);
            request.PreviousData.AddProperty<Data1, int>(x => x.Introduction, oldEntity.Introduction);

            request.Data.AddProperty<Data1, string>(x => x.Name, newEntity.Name);
            request.Data.AddProperty<Data1, int>(x => x.Introduction, newEntity.Introduction);

            var result = await controller.UpdateEntityForTesting(request).ConfigureAwait(true);

            Assert.NotNull(result);

            adapter.Verify(x => x.PrepareForWriting(source));

            var expected = newEntity;
            var actual = result;

            Assert.Equal(expected, actual);

            originalData[originalData.FindIndex(x => x.Id == 402)] = newEntity;

            Assert.Equal(originalData, data);
        }

        [Fact]
        public async Task FailsToUpdateSingleEntryWithPatch()
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
                .Setup(x => x.AllowUpdateOperations)
                .Returns(true);
            adapter.Setup(x => x.PrepareForReading(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();
            adapter
                .Setup(x => x.UpdateEntity(It.IsAny<Data1>()))
                .Returns<Data1>(x => x);

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new UpdateSingleEntityRequest<Data1>
            {
                Id = "402",
                PreviousData = new ExpandoObject(),
                Data = new ExpandoObject(),
            };

            var oldEntity = data.Single(x => x.Id == 402);
            var newEntity = oldEntity with
            {
                Name = "Aegon Targaryen",
                Introduction = 63,
            };

            request.PreviousData.AddProperty<Data1, string>(x => x.Name, newEntity.Name);
            request.PreviousData.AddProperty<Data1, int>(x => x.Introduction, newEntity.Introduction);

            request.Data.AddProperty<Data1, string>(x => x.Name, newEntity.Name);
            request.Data.AddProperty<Data1, int>(x => x.Introduction, newEntity.Introduction);

            var result = await controller.UpdateEntityForTesting(request).ConfigureAwait(true);

            Assert.Null(result);

            adapter.Verify(x => x.PrepareForWriting(source));
            adapter.Verify(x => x.ProviderType);
            adapter.VerifyNoOtherCalls();

            Assert.Equal(originalData, data);
        }
    }
}
