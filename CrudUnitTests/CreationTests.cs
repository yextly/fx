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
    public sealed class CreationTests
    {
        [Fact]
        public async Task CanCreateANewEntityWithPatching()
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
                .Setup(x => x.AllowCreateOperations)
                .Returns(true);
            adapter.Setup(x => x.PrepareForReading(It.IsAny<IQueryable<Data1>>()))
                .Throws<NotSupportedException>();
            adapter.Setup(x => x.CreateNewEntity(It.IsAny<Data1>()))
                .Returns<Data1>(x =>
                {
                    Assert.Equal(0, x.Id);
                    x.Id = 906;

                    data.Add(x);

                    return x;
                });
            adapter
                .Setup(x => x.ConstructEntity())
                .Returns(new Data1());

            var controller = new Data1Controller(logger, adapter.Object, source);
            var request = new CreateEntityRequest
            {
                Data = new ExpandoObject(),
            };

            var expected = new Data1
            {
                Id = 906,
                Introduction = 99,
                Name = "Viserion",
                Allegiances = "Dragons",
            };

            request.Data.AddProperty<Data1, string>(x => x.Name, expected.Name);
            request.Data.AddProperty<Data1, int>(x => x.Introduction, expected.Introduction);
            request.Data.AddProperty<Data1, string>(x => x.Allegiances, expected.Allegiances);

            var (entry, message) = await controller.CreateEntityForTesting(request).ConfigureAwait(true);

            Assert.NotNull(entry);
            Assert.Null(message);

            var actual = entry;

            Assert.Equal(expected, actual);

            originalData.Add(expected);

            Assert.Equal(originalData, data);
        }
    }
}
