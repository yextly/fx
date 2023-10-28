// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents an adapter for <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the identity to adapt.</typeparam>
    public interface ICrudDataAdapter<TEntity>
    {
        /// <summary>
        /// Indicates whether the adapter supports creation operations.
        /// </summary>
        bool AllowCreateOperations { get; }

        /// <summary>
        /// Indicates whether the adapter supports delete operations.
        /// </summary>
        bool AllowDeleteOperations { get; }

        /// <summary>
        /// Indicates whether the adapter supports update operations.
        /// </summary>
        bool AllowUpdateOperations { get; }

        /// <summary>
        /// Returns the type of the provider.
        /// </summary>
        ProviderType ProviderType { get; }

        /// <summary>
        /// Cronstructs an entity.
        /// </summary>
        /// <remarks>This method is the functional equivalent to a public parameterless constructor. The instance must be created in a way that is not attached to any context.</remarks>
        /// <returns>A newly constructed entity.</returns>
        TEntity ConstructEntity();

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <remarks>The execution must not be deferred.</remarks>
        /// <returns>The final effective created entity.</returns>
        TEntity CreateNewEntity(TEntity entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <remarks>The execution must not be deferred.</remarks>
        void Delete(TEntity entity);

        /// <summary>
        /// Prepares the source for <b>read only</b> operations.
        /// </summary>
        /// <param name="query">The source query.</param>
        /// <returns></returns>
        IQueryable<TEntity> PrepareForReading(IQueryable<TEntity> query);

        /// <summary>
        /// Prepares the source for <b>read and write</b> operations.
        /// </summary>
        /// <param name="query">The source query.</param>
        /// <returns></returns>
        IQueryable<TEntity> PrepareForWriting(IQueryable<TEntity> query);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The final effective updated entity.</returns>
        TEntity UpdateEntity(TEntity entity);
    }
}
