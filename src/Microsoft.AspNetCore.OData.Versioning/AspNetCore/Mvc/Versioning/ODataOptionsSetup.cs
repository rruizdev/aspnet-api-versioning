namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using Microsoft.AspNetCore.OData;
    using Microsoft.AspNetCore.OData.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OData;
    using Microsoft.OData.ModelBuilder;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the API versioning <see cref="IConfigureOptions{T}">configuration</see> to set up <see cref="ODataOptions">OData options</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class ODataOptionsSetup : IConfigureOptions<ODataOptions>
    {
        readonly IOptions<ODataApiVersioningOptions> versioningOptions;
        readonly IEnumerable<IModelConfiguration> modelConfigurations;
        readonly ODataRouteConfiguration routeConfiguration;
        readonly Func<ODataModelBuilder> modelBuilderFactory;
        readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataOptionsSetup"/> class.
        /// </summary>
        /// <param name="options">The <see cref="IOptions{TOptions}">holder</see> of <see cref="ODataApiVersioningOptions">options</see>.</param>
        /// <param name="modelConfigurations">The <see cref="IEnumerable{T}">sequence</see> of
        /// <see cref="IModelConfiguration">model configurations</see> used to create models.</param>
        /// <param name="routeConfiguration">The current <see cref="ODataRouteConfiguration">OData route configuration</see>.</param>
        /// <param name="modelBuilderFactory">The factory <see cref="Func{TResult}">function</see> used to create
        /// <see cref="ODataModelBuilder">OData model builders</see>.</param>
        /// <param name="serviceProvider">The associated <see cref="IServiceProvider">service provider</see>.</param>
        public ODataOptionsSetup(
            IOptions<ODataApiVersioningOptions> options,
            IEnumerable<IModelConfiguration> modelConfigurations,
            ODataRouteConfiguration routeConfiguration,
            Func<ODataModelBuilder> modelBuilderFactory,
            IServiceProvider serviceProvider )
        {
            versioningOptions = options;
            this.modelConfigurations = modelConfigurations;
            this.routeConfiguration = routeConfiguration;
            this.modelBuilderFactory = modelBuilderFactory;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public void Configure( ODataOptions options )
        {
            if ( options == null )
            {
                throw new ArgumentNullException( nameof( options ) );
            }

            foreach ( var configuration in versioningOptions.Value.Configurations )
            {
                var prefix = configuration.Key;
                var configureAction = configuration.Value;
                var builder = modelBuilderFactory();

                foreach ( var modelConfiguration in modelConfigurations )
                {
                    modelConfiguration.Apply( builder, ApiVersion.Neutral, prefix );
                }

                var model = builder.GetEdmModel();

                options.AddModel(
                    prefix,
                    model,
                    container =>
                    {
                        container.AddApiVersioning( prefix, serviceProvider );
                        configureAction?.Invoke( container );
                    } );

                var tuple = options.Models[prefix];
                var services = new EdmModelDecorator( tuple.Item2.WithParent( serviceProvider ) );

                model = new EdmModelProxy( tuple.Item1, routeConfiguration, services );
                options.Models[prefix] = (model, services);
            }
        }
    }
}