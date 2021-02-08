namespace Microsoft.AspNetCore.Mvc.ApiExplorer
{
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.OData;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an API explorer that provides <see cref="ApiDescription">API descriptions</see> for actions represented by
    /// <see cref="ControllerActionDescriptor">controller action descriptors</see> that are defined by
    /// <see cref="ODataController">OData controllers</see> and are <see cref="ApiVersion">API version</see> aware.
    /// </summary>
    [CLSCompliant( false )]
    public class ODataApiDescriptionProvider : IApiDescriptionProvider
    {
        const int AfterApiVersioning = -100;
        readonly IOptions<ODataApiExplorerOptions> options;
        readonly IOptions<ODataOptions> odataOptions;
        readonly Lazy<ModelMetadata> modelMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataApiDescriptionProvider"/> class.
        /// </summary>
        /// <param name="modelTypeProvider">The <see cref="IModelTypeBuilder">builder</see> used to create surrogate model types.</param>
        /// <param name="metadataProvider">The <see cref="IModelMetadataProvider">provider</see> used to retrieve model metadata.</param>
        /// <param name="defaultQuerySettings">The OData <see cref="DefaultQuerySettings">default query setting</see>.</param>
        /// <param name="options">The <see cref="IOptions{TOptions}">container</see> of configured <see cref="ODataApiExplorerOptions">API explorer options</see>.</param>
        /// <param name="odataOptions">A <see cref="IOptions{TOptions}">holder</see> containing the current <see cref="ODataOptions">OData options</see>.</param>
        public ODataApiDescriptionProvider(
            IModelTypeBuilder modelTypeProvider,
            IModelMetadataProvider metadataProvider,
            DefaultQuerySettings defaultQuerySettings,
            IOptions<ODataApiExplorerOptions> options,
            IOptions<ODataOptions> odataOptions )
        {
            ModelTypeBuilder = modelTypeProvider;
            MetadataProvider = metadataProvider;
            DefaultQuerySettings = defaultQuerySettings;
            this.options = options;
            this.odataOptions = odataOptions;
            modelMetadata = new Lazy<ModelMetadata>( NewModelMetadata );
        }

        /// <summary>
        /// Gets the model type builder used by the API explorer.
        /// </summary>
        /// <value>The associated <see cref="IModelTypeBuilder">model type builder</see>.</value>
        protected virtual IModelTypeBuilder ModelTypeBuilder { get; }

        /// <summary>
        /// Gets the model metadata provider associated with the description provider.
        /// </summary>
        /// <value>The <see cref="IModelMetadataProvider">provider</see> used to retrieve model metadata.</value>
        protected IModelMetadataProvider MetadataProvider { get; }

        /// <summary>
        /// Gets the OData default query settings.
        /// </summary>
        /// <value>The OData <see cref="DefaultQuerySettings">default query setting</see>.</value>
        protected DefaultQuerySettings DefaultQuerySettings { get; }

        /// <summary>
        /// Gets the options associated with the API explorer.
        /// </summary>
        /// <value>The current <see cref="ApiExplorerOptions">API explorer options</see>.</value>
        protected ODataApiExplorerOptions Options => options.Value;

        /// <summary>
        /// Gets the current OData options.
        /// </summary>
        /// <value>The current <see cref="ODataOptions">OData options</see>.</value>
        protected ODataOptions ODataOptions => odataOptions.Value;

        /// <summary>
        /// Gets the order precedence of the current API description provider.
        /// </summary>
        /// <value>The order precedence of the current API description provider. The default value is -100.</value>
        public virtual int Order => AfterApiVersioning;

        /// <summary>
        /// Occurs after the providers have been executed.
        /// </summary>
        /// <param name="context">The current <see cref="ApiDescriptionProviderContext">execution context</see>.</param>
        /// <remarks>The default implementation performs no action.</remarks>
        public virtual void OnProvidersExecuted( ApiDescriptionProviderContext context )
        {
            if ( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }

            var results = context.Results;
            var actionMapping = new Dictionary<string, ODataActionCollection>(
                capacity: ODataOptions.Models.Count,
                StringComparer.OrdinalIgnoreCase );

            for ( var i = 0; i < results.Count; i++ )
            {
                var result = results[i];
                var action = result.ActionDescriptor;
                var route = action.GetODataRoute();

                if ( route == null || !IsVisible( action ) )
                {
                    continue;
                }

                var apiVersion = route.Model.GetAnnotationValue<ApiVersionAnnotation>( route.Model ).ApiVersion;
                var serviceProvider = ODataOptions.GetODataServiceProvider( route.Prefix );

                if ( actionMapping.TryGetValue( route.Prefix, out var actions ) )
                {
                    actions.Add( result );
                }
                else
                {
                    var uriResolver = serviceProvider.GetRequiredService<ODataUriResolver>();
                    actionMapping.Add( route.Prefix, new ODataActionCollection( result, uriResolver ) );
                }

                for ( var j = 0; j < result.ParameterDescriptions.Count; j++ )
                {
                    var parameter = result.ParameterDescriptions[j];
                    var originalType = parameter.Type;

                    if ( originalType == null )
                    {
                        continue;
                    }

                    var typeContext = new TypeSubstitutionContext( serviceProvider, ModelTypeBuilder, apiVersion );
                    var newType = originalType.SubstituteIfNecessary( typeContext );

                    parameter.Type = newType;
                    parameter.ModelMetadata = MetadataProvider.GetMetadataForType( originalType ).SubstituteIfNecessary( newType );
                }

                for ( var j = 0; j < result.SupportedResponseTypes.Count; j++ )
                {
                    var responseType = result.SupportedResponseTypes[j];
                    var originalType = responseType.Type;

                    if ( originalType == null )
                    {
                        continue;
                    }

                    var typeContext = new TypeSubstitutionContext( serviceProvider, ModelTypeBuilder, apiVersion );
                    var newType = originalType.SubstituteIfNecessary( typeContext );

                    responseType.Type = newType;
                    responseType.ModelMetadata = MetadataProvider.GetMetadataForType( originalType ).SubstituteIfNecessary( newType );
                }
            }

            foreach ( var actions in actionMapping.Values )
            {
                ExploreQueryOptions( actions, actions.UriResolver );
            }
        }

        /// <summary>
        /// Occurs when the providers are being executed.
        /// </summary>
        /// <param name="context">The current <see cref="ApiDescriptionProviderContext">execution context</see>.</param>
        /// <remarks>The default implementation performs no operation.</remarks>
        public virtual void OnProvidersExecuting( ApiDescriptionProviderContext context )
        {
        }

        /// <summary>
        /// Returns a value indicating whether the specified action is visible to the API Explorer.
        /// </summary>
        /// <param name="action">The <see cref="ActionDescriptor">action</see> to evaluate.</param>
        /// <returns>True if the <paramref name="action"/> is visible; otherwise, false.</returns>
        protected bool IsVisible( ActionDescriptor action )
        {
            var ignoreApiExplorerSettings = !Options.UseApiExplorerSettings;

            if ( ignoreApiExplorerSettings )
            {
                return true;
            }

            return action.GetProperty<ApiExplorerModel>()?.IsVisible ??
                   action.GetProperty<ControllerModel>()?.ApiExplorer?.IsVisible ??
                   false;
        }

        /// <summary>
        /// Populates the API version parameters for the specified API description.
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription">API description</see> to populate parameters for.</param>
        /// <param name="apiVersion">The <see cref="ApiVersion">API version</see> used to populate parameters with.</param>
        protected virtual void PopulateApiVersionParameters( ApiDescription apiDescription, ApiVersion apiVersion )
        {
            var parameterSource = Options.ApiVersionParameterSource;
            var context = new ApiVersionParameterDescriptionContext( apiDescription, apiVersion, modelMetadata.Value, Options );

            parameterSource.AddParameters( context );
        }

        /// <summary>
        /// Explores the OData query options for the specified API descriptions.
        /// </summary>
        /// <param name="apiDescriptions">The <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="ApiDescription">API descriptions</see> to explore.</param>
        /// <param name="uriResolver">The associated <see cref="ODataUriResolver">OData URI resolver</see>.</param>
        protected virtual void ExploreQueryOptions( IReadOnlyList<ApiDescription> apiDescriptions, ODataUriResolver uriResolver )
        {
            if ( uriResolver == null )
            {
                throw new ArgumentNullException( nameof( uriResolver ) );
            }

            var queryOptions = Options.QueryOptions;
            var settings = new ODataQueryOptionSettings()
            {
                NoDollarPrefix = uriResolver.EnableNoDollarQueryOptions,
                DescriptionProvider = queryOptions.DescriptionProvider,
                DefaultQuerySettings = DefaultQuerySettings,
                ModelMetadataProvider = MetadataProvider,
            };

            queryOptions.ApplyTo( apiDescriptions, settings );
        }

        ModelMetadata NewModelMetadata() => new ApiVersionModelMetadata( MetadataProvider, Options.DefaultApiVersionParameterDescription );

        sealed class ODataActionCollection : IReadOnlyList<ApiDescription>
        {
            readonly List<ApiDescription> items = new List<ApiDescription>();

            internal ODataActionCollection( ApiDescription item, ODataUriResolver uriResolver )
            {
                items.Add( item );
                UriResolver = uriResolver;
            }

            internal ODataUriResolver UriResolver { get; }

            public int Count => items.Count;

            public ApiDescription this[int index] => items[index];

            public IEnumerator<ApiDescription> GetEnumerator() => items.GetEnumerator();

            internal void Add( ApiDescription item ) => items.Add( item );

            IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
        }
    }
}