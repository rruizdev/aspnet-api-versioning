namespace Microsoft.AspNetCore.OData
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.OData.Abstracts;
    using Microsoft.AspNetCore.OData.Routing.Conventions;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using Microsoft.OData.ModelBuilder;
    using System;
    using System.Linq;
    using static Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

    /// <summary>
    /// Provides extension methods for the <see cref="IODataBuilder"/> interface.
    /// </summary>
    [CLSCompliant( false )]
    public static class IODataBuilderExtensions
    {
        /// <summary>
        /// Enables service API versioning for the specified OData configuration.
        /// </summary>
        /// <param name="builder">The <see cref="IODataBuilder">OData builder</see> available in the application.</param>
        /// <returns>The original <paramref name="builder"/> object.</returns>
        public static IODataBuilder EnableApiVersioning( this IODataBuilder builder )
        {
            if ( builder == null )
            {
                throw new ArgumentNullException( nameof( builder ) );
            }

            AddODataServices( builder.Services );
            return builder;
        }

        /// <summary>
        /// Enables service API versioning for the specified OData configuration.
        /// </summary>
        /// <param name="builder">The <see cref="IODataBuilder">OData builder</see> available in the application.</param>
        /// <param name="setupAction">An <see cref="Action{T}">action</see> used to configure the provided options.</param>
        /// <returns>The original <paramref name="builder"/> object.</returns>
        public static IODataBuilder EnableApiVersioning( this IODataBuilder builder, Action<ODataApiVersioningOptions> setupAction )
        {
            if ( builder == null )
            {
                throw new ArgumentNullException( nameof( builder ) );
            }

            var services = builder.Services;

            AddODataServices( services );
            services.Configure( setupAction );

            return builder;
        }

        static void AddODataServices( IServiceCollection services )
        {
            var partManager = services.GetService<ApplicationPartManager>();

            if ( partManager == null )
            {
                partManager = new ApplicationPartManager();
                services.TryAddSingleton( partManager );
            }

            partManager.ApplicationParts.Add( new AssemblyPart( typeof( IODataBuilderExtensions ).Assembly ) );

            ConfigureDefaultFeatureProviders( partManager );

            services.AddHttpContextAccessor();
            services.TryAddSingleton<ODataRouteConfiguration>();
            services.TryAdd( Transient<VersionedODataModelBuilder, VersionedODataModelBuilder>() );
            services.AddTransient<IApplicationModelProvider, ODataRouteConfigurationScope>();
            services.AddTransient<Func<ODataModelBuilder>>( sp => () => new ODataConventionModelBuilder() );
            services.TryAddEnumerable( Transient<IApiControllerSpecification, ODataControllerSpecification>() );
            services.TryAddEnumerable( Transient<IConfigureOptions<ODataOptions>, ODataOptionsSetup>() );
            services.TryAddEnumerable( Singleton<MatcherPolicy, DefaultMetadataMatcherPolicy>() );
            services.TryReplaceEnumerable( Transient<IApplicationModelProvider, ODataApplicationModelProvider>(), typeof( ApiVersioningApplicationModelProvider ) );
            services.TryReplaceEnumerable( Transient<IODataControllerActionConvention, VersionedMetadataRoutingConvention>(), typeof( MetadataRoutingConvention ) );
            services.AddModelConfigurationsAsServices( partManager );
        }

        static T GetService<T>( this IServiceCollection services ) => (T) services.LastOrDefault( d => d.ServiceType == typeof( T ) )?.ImplementationInstance!;

        static void TryReplaceEnumerable( this IServiceCollection services, ServiceDescriptor descriptor, Type oldImplementationType )
        {
            var found = false;

            for ( var i = 0; i < services.Count; i++ )
            {
                var service = services[i];

                if ( service.ServiceType == descriptor.ServiceType &&
                     service.ImplementationType == oldImplementationType )
                {
                    services[i] = descriptor;
                    found = true;
                    break;
                }
            }

            if ( !found )
            {
                services.Add( descriptor );
            }
        }

        static void AddModelConfigurationsAsServices( this IServiceCollection services, ApplicationPartManager partManager )
        {
            var feature = new ModelConfigurationFeature();
            var modelConfigurationType = typeof( IModelConfiguration );

            partManager.PopulateFeature( feature );

            foreach ( var modelConfiguration in feature.ModelConfigurations.Select( t => t.AsType() ) )
            {
                services.TryAddEnumerable( Transient( modelConfigurationType, modelConfiguration ) );
            }
        }

        static void ConfigureDefaultFeatureProviders( ApplicationPartManager partManager )
        {
            if ( !partManager.FeatureProviders.OfType<ModelConfigurationFeatureProvider>().Any() )
            {
                partManager.FeatureProviders.Add( new ModelConfigurationFeatureProvider() );
            }
        }
    }
}