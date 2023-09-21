// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Yextly.ServiceFabric.Mvc.Crud;

namespace CrudUnitTests
{
    internal sealed class Data1Controller : CrudResourceControllerBase<Data1, Data1>
    {
        public Data1Controller(ILogger<Data1Controller> logger, ICrudDataAdapter<Data1> adapter, IQueryable<Data1> entities) : base(logger, adapter, entities)
        {
        }

        public Task<CrudCollectionResult<Data1>> GetListForTesting(GetListRequest request)
        {
            return base.GetListInternal(request);
        }

        public Task<CrudCollectionResult<Data1>> GetMultipleForTesting(GetMultipleEntitiesRequest request)
        {
            return base.GetMultipleInternal(request, true);
        }

        public Task<Data1?> UpdateEntityForTesting(UpdateSingleEntityRequest<Data1> request)
        {
            return base.UpdateEntityInternal(request);
        }

        public Task<Data1> DeleteEntityForTesting(DeleteEntityRequest<Data1> request)
        {
            return base.DeleteEntityInternal(request);
        }

        public Task<(Data1? Entity, string? Message)> CreateEntityForTesting(CreateEntityRequest request)
        {
            return base.CreateEntityInternal(request);
        }

        public Task<AffectedItemsResponse> UpdateEntitiesForTesting(UpdateMultipleEntitiesRequest<Data1> request)
        {
            return base.UpdateEntitiesInternal(request);
        }
    }
}
