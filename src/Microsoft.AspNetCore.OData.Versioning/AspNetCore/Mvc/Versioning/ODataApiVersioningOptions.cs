namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using Microsoft.AspNetCore.OData.Batch;
    using Microsoft.OData;
    using System;
    using System.Collections.Generic;
    using static Microsoft.OData.ServiceLifetime;

    /// <summary>
    /// Represents the possible API versioning options for OData services.
    /// </summary>
    public class ODataApiVersioningOptions
    {
        readonly Dictionary<string, Action<IContainerBuilder>> configurations =
            new Dictionary<string, Action<IContainerBuilder>>( StringComparer.OrdinalIgnoreCase );

        /// <summary>
        /// Gets the collection of model configurations.
        /// </summary>
        /// <value>The <see cref="IReadOnlyDictionary{TKey, TValue}">read-only collection</see> of OData configurations.</value>
        public IReadOnlyDictionary<string, Action<IContainerBuilder>> Configurations => configurations;

        /// <summary>
        /// Adds an OData configuration for the provided prefix.
        /// </summary>
        /// <returns>The original <see cref="ODataApiVersioningOptions">options</see>.</returns>
        public ODataApiVersioningOptions AddModels() => AddModels( string.Empty, _ => { } );

        /// <summary>
        /// Adds an OData configuration for the provided prefix.
        /// </summary>
        /// <param name="prefix">The associated OData prefix.</param>
        /// <returns>The original <see cref="ODataApiVersioningOptions">options</see>.</returns>
        public virtual ODataApiVersioningOptions AddModels( string prefix ) => AddModels( prefix, _ => { } );

        /// <summary>
        /// Adds an OData configuration for the provided prefix.
        /// </summary>
        /// <param name="configureAction">The configuration <see cref="Action{T}">action</see>.</param>
        /// <returns>The original <see cref="ODataApiVersioningOptions">options</see>.</returns>
        public virtual ODataApiVersioningOptions AddModels( Action<IContainerBuilder> configureAction ) =>
            AddModels( string.Empty, configureAction );

        /// <summary>
        /// Adds an OData configuration for the provided prefix.
        /// </summary>
        /// <param name="prefix">The associated OData prefix.</param>
        /// <param name="batchHandler">The <see cref="ODataBatchHandler">$batch handler</see>.</param>
        /// <returns>The original <see cref="ODataApiVersioningOptions">options</see>.</returns>
        [CLSCompliant( false )]
        public ODataApiVersioningOptions AddModels( string prefix, ODataBatchHandler batchHandler ) =>
            AddModels( prefix, builder => builder.AddService( Singleton, sp => batchHandler ) );

        /// <summary>
        /// Adds an OData configuration for the provided prefix.
        /// </summary>
        /// <param name="batchHandler">The <see cref="ODataBatchHandler">$batch handler</see>.</param>
        /// <returns>The original <see cref="ODataApiVersioningOptions">options</see>.</returns>
        [CLSCompliant( false )]
        public ODataApiVersioningOptions AddModels( ODataBatchHandler batchHandler ) =>
            AddModels( string.Empty, builder => builder.AddService( Singleton, sp => batchHandler ) );

        /// <summary>
        /// Adds an OData configuration for the provided prefix.
        /// </summary>
        /// <param name="prefix">The associated OData prefix.</param>
        /// <param name="configureAction">The configuration <see cref="Action{T}">action</see>.</param>
        /// <returns>The original <see cref="ODataApiVersioningOptions">options</see>.</returns>
        public virtual ODataApiVersioningOptions AddModels( string prefix, Action<IContainerBuilder> configureAction )
        {
            configurations.Add( prefix, configureAction );
            return this;
        }
    }
}