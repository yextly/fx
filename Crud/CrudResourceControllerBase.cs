// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents a common controller to provide CRUD-like operations on the entity <typeparamref name="TInnerEntity"/>.
    /// </summary>
    /// <typeparam name="TInnerEntity">The type of the (inner) entity.</typeparam>
    /// <typeparam name="TOuterEntity">The type of the (outer) entity.</typeparam>
    /// <remarks><typeparamref name="TOuterEntity"/> can be a subset of <typeparamref name="TInnerEntity"/> but they must have the same shape.
    /// This cannot be enforced by the controller as it would introduce useless bounds.
    /// </remarks>
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [UseRoleProvider(typeof(CrudResourceRoleProvider))]
    public abstract partial class CrudResourceControllerBase<TInnerEntity, TOuterEntity> : ControllerBase where TInnerEntity : class where TOuterEntity : class
    {
        private const string OperationNotAllowedMessage = "The operation is not supported";

        private readonly ICrudDataAdapter<TInnerEntity> _adapter;
        private readonly IQueryable<TInnerEntity> _entities;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="logger">A logger instance.</param>
        /// <param name="adapter">The adapter to use.</param>
        /// <param name="entities">The data source as queryable.</param>
        protected CrudResourceControllerBase(ILogger<CrudResourceControllerBase<TInnerEntity, TOuterEntity>> logger, ICrudDataAdapter<TInnerEntity> adapter, IQueryable<TInnerEntity> entities)
        {
            _logger = logger;
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("create")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Create)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        public async Task<IActionResult> CreateEntity([FromBody] CreateEntityRequest request)
        {
            try
            {
                if (!_adapter.AllowCreateOperations)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = CrudResourceControllerBase<TInnerEntity, TOuterEntity>.OperationNotAllowedMessage,
                    });
                }

                var (ret, message) = await CreateEntityInternal(request).ConfigureAwait(false);

                if (ret == null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = message ?? "Not supported.",
                    });
                }
                else
                {
                    return Ok(ConvertToOuterEntity(ret));
                }
            }
            catch (Exception ex)
            {
                LogCannotCreateEntity(ex.Message, ex);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Deletes a new entity.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("delete")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        public async Task<IActionResult> DeleteEntity([FromBody] DeleteEntityRequest<TInnerEntity> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                if (!_adapter.AllowDeleteOperations)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = CrudResourceControllerBase<TInnerEntity, TOuterEntity>.OperationNotAllowedMessage,
                    });
                }

                var ret = await DeleteEntityInternal(request).ConfigureAwait(false);

                return Ok(ConvertToOuterEntity(ret));
            }
            catch (Exception ex)
            {
                LogFailedToDeleteTheEntity(request.Id, ex.Message, ex);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("delete-many")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "This is not an issue")]
        public async Task<IActionResult> DeleteManyEntities([FromBody] DeleteMultipleEntitiesRequest<TInnerEntity> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                if (!_adapter.AllowDeleteOperations)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = CrudResourceControllerBase<TInnerEntity, TOuterEntity>.OperationNotAllowedMessage,
                    });
                }

                var r = new DeleteEntityRequest<TInnerEntity>();
                foreach (var item in request.Ids)
                {
                    r.Id = item;
                    _ = await DeleteEntityInternal(r).ConfigureAwait(false);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete the entities: {Message}", ex.Message);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Gets the list of the provided entities.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("get-list")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(CrudCollectionResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Read)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        public async Task<IActionResult> GetList([FromBody] GetListRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                var ret = await GetListInternal(request).ConfigureAwait(false);

                return Ok(ConvertToOuterEntity(ret));
            }
            catch (Exception ex)
            {
                LogFailedToGetTheEntities(ex.Message, ex);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Gets multiple entities.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("get-many")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Read)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        public async Task<IActionResult> GetMultiple([FromBody] GetMultipleEntitiesRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                var ret = await GetMultipleInternal(request, true).ConfigureAwait(false);

                return Ok(ConvertToOuterEntity(ret));
            }
            catch (Exception ex)
            {
                LogFailedToGetTheEntities(ex.Message, ex);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("get-single")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Read)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        public async Task<IActionResult> GetSingle(string id)
        {
            try
            {
                var t = await GetMultipleInternal(new GetMultipleEntitiesRequest { Ids = [id] }, true).ConfigureAwait(false);

                if (t.Data.Count == 1)
                {
                    return Ok(ConvertToOuterEntity(t.Data[0]));
                }
                else
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Cannot find the entity."
                    });
                }
            }
            catch (Exception ex)
            {
                LogFailedToGetTheEntity(id, ex.Message, ex);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Updates multipl entities.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("update-many")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(AffectedItemsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        public async Task<IActionResult> UpdateEntities([FromBody] UpdateMultipleEntitiesRequest<TInnerEntity> request)
        {
            try
            {
                if (!_adapter.AllowUpdateOperations)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = CrudResourceControllerBase<TInnerEntity, TOuterEntity>.OperationNotAllowedMessage,
                    });
                }

                var response = await UpdateEntitiesInternal(request).ConfigureAwait(false);

                return Ok(response);
            }
            catch (Exception ex)
            {
                LogFailedToUpdateTheEntities(ex.Message, ex);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Updates a single entity.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("update")]
        [RoutePolicy(KnownPolicies.Authenticated)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [CrudOperationType(OperationType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is logged")]
        public async Task<IActionResult> UpdateEntity([FromBody] UpdateSingleEntityRequest<TInnerEntity> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                if (!_adapter.AllowUpdateOperations)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = CrudResourceControllerBase<TInnerEntity, TOuterEntity>.OperationNotAllowedMessage,
                    });
                }

                var ret = await UpdateEntityInternal(request).ConfigureAwait(false);

                if (ret == null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "The entity has changed meanwhile or does not exist.",
                    });
                }
                else
                {
                    return Ok(ConvertToOuterEntity(ret));
                }
            }
            catch (Exception ex)
            {
                LogFailedToUpdateTheEntity(request.Id, ex.Message, ex);

                return BadRequest(GenerateErrorDto(ex));
            }
        }

        /// <summary>
        /// Creates an new entity.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The exception is captured and returned")]
        protected virtual Task<(TInnerEntity? Entity, string? Message)> CreateEntityInternal(CreateEntityRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var entity = _adapter.ConstructEntity() ?? throw new InvalidOperationException("Entity constructor returned null.");

            if (request.Data != null)
            {
                var fields = (IDictionary<string, object?>)request.Data;

                foreach (var item in fields)
                {
                    var property = ReflectionHelpers.GetPropertyByName<TInnerEntity>(item.Key);

                    if (property == null)
                    {
                        string? m = $@"Invalid property name ""{item}"".";
                        entity = null;
                        return Task.FromResult((entity, (string?)m));
                    }
                    property.SetValue(entity, ExtractValue(item.Value, property.PropertyType));
                }
            }

            TInnerEntity? e;

            string? message = null;
            try
            {
                e = _adapter.CreateNewEntity(entity);
            }
            catch (Exception ex)
            {
                LogCannotCreateNewEntity(ex.Message, ex);

                message = ex.Message;
                e = null;
            }

            return Task.FromResult((e, message));
        }

        /// <summary>
        /// Provides the default <see cref="DeleteEntity"/> implementation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns the deleted entity.</returns>
        /// <exception cref="InvalidOperationException">Incorrect data or adapter errors.</exception>
        protected virtual async Task<TInnerEntity> DeleteEntityInternal(DeleteEntityRequest<TInnerEntity> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            // the contract here is that we return the deleted item, therefore we must fetch it first.
            // at the moment we do not check previous data. we probably ought to prevent concurreny issues, but this will be in the future

            var t = await GetMultipleInternal(new GetMultipleEntitiesRequest { Ids = [request.Id] }, false).ConfigureAwait(false);

            if (t.Data.Count == 0)
            {
                return _adapter.ConstructEntity() ?? throw new InvalidOperationException("Entity constructor returned null.");
            }
            else if (t.Data.Count > 1)
            {
                // this should never happen
                throw new InvalidOperationException("Expected at worst one record.");
            }
            else
            {
                //var source = _adapter.PrepareForWriting(_entities);
                //var keyProperty = RestControllerBase<TEntity>.GetNonCompositePrimaryKey();

                //var s = Expression.Parameter(typeof(TEntity), "source");
                //var p = Expression.Property(s, keyProperty.Name);

                //var value = Expression.Constant(CreateValueForExactMatch(request.Id, keyProperty.PropertyType), keyProperty.PropertyType);
                //var operation = Expression.Equal(p, value);
                //var f = Expression.Lambda<Func<TEntity, bool>>(operation, s);

                var e = t.Data[0];

                _adapter.Delete(e);

                return e;
            }
        }

        /// <summary>
        /// Provides the default <see cref="GetList"/> implementation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual Task<CrudCollectionResult<TInnerEntity>> GetListInternal(GetListRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var source = _adapter.PrepareForReading(_entities);

            if (request.Filter != null)
            {
                foreach (var item in request.Filter)
                {
                    if (string.IsNullOrWhiteSpace(item.Name) || string.IsNullOrWhiteSpace(item.Value))
                    {
                        continue;
                    }

                    var s = Expression.Parameter(typeof(TInnerEntity), "source");
                    var property = ReflectionHelpers.GetPropertyByName<TInnerEntity>(item.Name);

                    var p = Expression.Property(s, property);

                    var value = Expression.Constant(CreateValueForComparison(item.Value, property.PropertyType), property.PropertyType);

                    Expression operation;
                    if (property.PropertyType == typeof(string))
                    {
                        if (_adapter.ProviderType == ProviderType.EntityFramework)
                        {
                            var context = Expression.Constant(EF.Functions, typeof(DbFunctions));
                            operation = Expression.Call(ReflectionHelpers.GetEFLikeMethod(), context, p, value);
                        }
                        else
                        {
                            operation = Expression.Call(ReflectionHelpers.GetSearchHelperLikeMethod(), p, value);
                        }
                    }
                    else
                    {
                        operation = Expression.Equal(p, value);
                    }

                    var f = Expression.Lambda<Func<TInnerEntity, bool>>(operation, s);
                    source = source.Where(f);
                }
            }

            var filteredOnly = source;

            if (request.Sorting != null)
            {
                foreach (var item in request.Sorting)
                {
                    if (string.IsNullOrWhiteSpace(item.Name))
                        continue;

                    source = item.SortType switch
                    {
                        SortType.Descending => source.OrderByDescending(item.Name),
                        _ => source.OrderBy(item.Name),
                    };
                }
            }

            if (request.Range != null)
            {
                var start = request.Range.Start;
                var end = request.Range.End;

                if (start < 0)
                    start = 0;
                if (end < start)
                    end = start;

                source = PagingUtilities.Page(source, start, end - start + 1);
            }

            var count = filteredOnly.Count();
            var data = source.ToList();

            var ret = new CrudCollectionResult<TInnerEntity>
            {
                Count = count,
                Data = data,
            };

            return Task.FromResult(ret);
        }

        /// <summary>
        /// Provides the default <see cref="GetSingle"/> implementation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="isReading">Indicates if we are in a readonly context.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual Task<CrudCollectionResult<TInnerEntity>> GetMultipleInternal(GetMultipleEntitiesRequest request, bool isReading)
        {
            if (request == null || request.Ids == null || request.Ids.Length == 0)
                return Task.FromResult(new CrudCollectionResult<TInnerEntity>());

            var source = (isReading) ? _adapter.PrepareForReading(_entities) : _adapter.PrepareForWriting(_entities);

            var keyProperty = GetNonCompositePrimaryKey();

            var s = Expression.Parameter(typeof(TInnerEntity), "source");
            var p = Expression.Property(s, keyProperty.Name);

            var ids = request.Ids;
            if (ids.Length == 1)
            {
                var value = Expression.Constant(CreateValueForExactMatch(ids[0], keyProperty.PropertyType), keyProperty.PropertyType);
                var operation = Expression.Equal(p, value);
                var f = Expression.Lambda<Func<TInnerEntity, bool>>(operation, s);
                source = source.Where(f);
            }
            else
            {
                var list = CreateList(keyProperty.PropertyType);
                foreach (var item in ids)
                {
                    list.Add(CreateValueForExactMatch(item, keyProperty.PropertyType));
                }
                var listExpression = Expression.Constant(list, list.GetType());

                var containsMethod = GetConstainsMethod(keyProperty.PropertyType);
                var operation = Expression.Call(containsMethod, listExpression, p);
                var f = Expression.Lambda<Func<TInnerEntity, bool>>(operation, s);
                source = source.Where(f);
            }

            var data = source.ToList();

            var ret = new CrudCollectionResult<TInnerEntity>
            {
                Count = -1,
                Data = data,
            };

            return Task.FromResult(ret);
        }

        /// <summary>
        /// Projects the entity.
        /// </summary>
        /// <param name="entity">The source entity.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual TOuterEntity ProjectEntity(TInnerEntity entity)
        {
            if (entity is TOuterEntity r)
                return r;
            else
                throw new InvalidOperationException($"The conversion cannot take place automatically. You must implement {nameof(ProjectEntity)}.");
        }

        /// <summary>
        /// Provides the default <see cref="UpdateEntities"/> implementation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected virtual async Task<AffectedItemsResponse> UpdateEntitiesInternal(UpdateMultipleEntitiesRequest<TInnerEntity> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var r = new UpdateSingleEntityRequest<TInnerEntity>
            {
                Data = request.Data
            };

            var affected = new List<string>(request.Ids.Length);

            foreach (var item in request.Ids)
            {
                r.Id = item;
                var e = await UpdateEntityInternal(r).ConfigureAwait(false);
                if (e != null)
                    affected.Add(item);
            }

            var response = new AffectedItemsResponse
            {
                Ids = affected.ToArray(),
            };
            return response;
        }

        /// <summary>
        /// Provides the default <see cref="UpdateEntity"/> implementation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected virtual async Task<TInnerEntity?> UpdateEntityInternal(UpdateSingleEntityRequest<TInnerEntity> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var source = _adapter.PrepareForWriting(_entities);
            var s = Expression.Parameter(typeof(TInnerEntity));

            Expression conditions;
            {
                var keyProperty = GetNonCompositePrimaryKey();

                var p = Expression.Property(s, keyProperty.Name);

                var keyValue = Expression.Constant(CreateValueForExactMatch(request.Id, keyProperty.PropertyType), keyProperty.PropertyType);
                conditions = Expression.Equal(p, keyValue);
            }

            if (request.PreviousData != null)
            {
                var fields = (IDictionary<string, object?>)request.PreviousData;

                foreach (var item in fields)
                {
                    var property = ReflectionHelpers.GetPropertyByName<TInnerEntity>(item.Key);
                    var value = Expression.Constant(CreateValueForExactMatchFromObject(ExtractValue(item.Value, property.PropertyType), property.PropertyType), property.PropertyType);
                    var p = Expression.Property(s, property.Name);
                    var operation = Expression.Equal(p, value);

                    conditions = Expression.AndAlso(conditions, operation);
                }
            }

            var f = Expression.Lambda<Func<TInnerEntity, bool>>(conditions, s);

            var e = await FirstOrDefaultAsync(source, f).ConfigureAwait(false);

            if (e == null)
                return null;

            var newFields = (IDictionary<string, object?>)request.Data;
            foreach (var item in newFields)
            {
                var property = ReflectionHelpers.GetPropertyByName<TInnerEntity>(item.Key);
                var value = CreateValueForExactMatchFromObject(ExtractValue(item.Value, property.PropertyType), property.PropertyType);

                property.SetValue(e, value, null);
            }

            return _adapter.UpdateEntity(e);
        }

        private static IList CreateList(Type propertyType)
        {
            return (IList)(Activator.CreateInstance(typeof(List<>).MakeGenericType([propertyType])) ?? throw new InvalidOperationException());
        }

        private static object? CreateValueForExactMatch(string value, Type propertyType)
        {
            return TypeConverter.ConvertFromObject(value, propertyType);
        }

        private static object? CreateValueForExactMatchFromObject(object? value, Type propertyType)
        {
            if (value == null)
                return value;

            return TypeConverter.ConvertFromObject(value, propertyType);

            //if (propertyType.IsEnum)
            //    return Enum.ToObject(propertyType, value);
            //else if (propertyType == typeof(Guid))
            //    return Guid.Parse(value.ToString() ?? string.Empty);
            //else
            //    return Convert.ChangeType(value, propertyType);
        }

        private static object? ExtractValue(object? value, Type newType)
        {
            object? result = value;
            if (value is JsonElement e)
            {
                result = e.ValueKind switch
                {
                    JsonValueKind.Undefined => null,// currently we collapse this into 'null'
                    JsonValueKind.Object or JsonValueKind.Array => value,// currently we do not know what to do
                    JsonValueKind.String => e.GetString(),
                    JsonValueKind.Number => e.GetInt32(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => value,// currently we do not know what to do
                };
            }

            return TypeConverter.ConvertFromObject(result, newType);
        }

        private static ErrorResponse GenerateErrorDto(Exception ex)
        {
            string message;
            if (ex is ValidationException v)
                message = v.Message;
            else
                message = "Validation error.";

            return new ErrorResponse
            {
                Message = message,
            };
        }

        private static MethodInfo GetConstainsMethod(Type type)
        {
            //we look for: public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value

            return typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(x => x.Name == nameof(Enumerable.Contains) && x.GetParameters().Length == 2).MakeGenericMethod(type);
        }

        private static PropertyInfo GetNonCompositePrimaryKey()
        {
            // at the moment we use only data annotation. no ef conventions as we cannot known where the context comes from.

            var properties = typeof(TInnerEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var candidate = properties.SingleOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null);

            if (candidate == null)
            {
                // we have no data annotation (it happens in the fabric) so, for now, we check for the Id property

                const string KeyName = "Id";

                candidate = Array.Find(properties, x => string.Equals(x.Name, KeyName, StringComparison.OrdinalIgnoreCase));

                if (candidate == null)
                    throw new InvalidOperationException("Cannot infer the key of the entity. Either no annotation is available or no convention is used.");
                else
                    return candidate;
            }
            else
            {
                return candidate;
            }
        }

        private TOuterEntity ConvertToOuterEntity(TInnerEntity entity)
        {
            return ProjectEntity(entity);
        }

        private CrudCollectionResult<TOuterEntity> ConvertToOuterEntity(CrudCollectionResult<TInnerEntity> entities)
        {
            return new CrudCollectionResult<TOuterEntity>
            {
                Count = entities.Count,
                Data = entities.Data.ConvertAll(x => ProjectEntity(x)),
            };
        }

        private object? CreateValueForComparison(string value, Type propertyType)
        {
            // the Contains or Like operators have a meaning on string types only. For the rest, we use exact match (for now).
            if (_adapter.ProviderType == ProviderType.EntityFramework && propertyType == typeof(string))
                return "%" + value + "%";
            else
                return TypeConverter.ConvertFromObject(value, propertyType);
        }

        private async Task<TInnerEntity?> FirstOrDefaultAsync(IQueryable<TInnerEntity> source, Expression<Func<TInnerEntity, bool>> predicate)
        {
            if (_adapter.ProviderType == ProviderType.EntityFramework)
                return await source.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
            else
                return source.FirstOrDefault(predicate);
        }

        [LoggerMessage(Message = "Failed to create the entity: {Message}", Level = LogLevel.Warning, EventId = 1)]
        private partial void LogCannotCreateEntity(string message, Exception exception);

        [LoggerMessage(Message = "Failed to create a new entity: {Message}", Level = LogLevel.Warning, EventId = 7)]
        private partial void LogCannotCreateNewEntity(string message, Exception exception);

        [LoggerMessage(Message = "Failed to delete the entity {Id}: {Message}", Level = LogLevel.Warning, EventId = 2)]
        private partial void LogFailedToDeleteTheEntity(string id, string message, Exception exception);

        [LoggerMessage(Message = "Failed to get the entities: {Message}", Level = LogLevel.Warning, EventId = 3)]
        private partial void LogFailedToGetTheEntities(string message, Exception exception);

        [LoggerMessage(Message = "Failed to get the entity {Id}: {Message}", Level = LogLevel.Warning, EventId = 4)]
        private partial void LogFailedToGetTheEntity(string id, string message, Exception exception);

        [LoggerMessage(Message = "Failed to update the entities: {Message}", Level = LogLevel.Warning, EventId = 6)]
        private partial void LogFailedToUpdateTheEntities(string message, Exception exception);

        [LoggerMessage(Message = "Failed to update the entity {Id}: {Message}", Level = LogLevel.Warning, EventId = 5)]
        private partial void LogFailedToUpdateTheEntity(string id, string message, Exception exception);

        //private MethodInfo GetGenericComparer()
        //{
        //    return typeof(ReflectionHelpers).GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public).Single(x => x.Name == nameof(ReflectionHelpers.EqualsInternal));

        //    //return this.GetType().GetMethod(nameof(EqualsInternal), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic) ?? throw new InvalidOperationException();
        //}
    }
}
