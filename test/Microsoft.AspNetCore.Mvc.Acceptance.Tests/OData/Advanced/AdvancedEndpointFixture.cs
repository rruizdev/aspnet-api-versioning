namespace Microsoft.AspNetCore.OData.Advanced
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.OData.Advanced.Controllers.Endpoint;
    using Microsoft.AspNetCore.OData.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using Microsoft.OData.ModelBuilder;
    using System.Reflection;
    using static Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

    public class AdvancedEndpointFixture : ODataFixture
    {
        public AdvancedEndpointFixture()
        {
            EnableEndpointRouting = true;
            FilteredControllerTypes.Add( typeof( OrdersController ).GetTypeInfo() );
            FilteredControllerTypes.Add( typeof( Orders2Controller ).GetTypeInfo() );
            FilteredControllerTypes.Add( typeof( Orders3Controller ).GetTypeInfo() );
            FilteredControllerTypes.Add( typeof( PeopleController ).GetTypeInfo() );
            FilteredControllerTypes.Add( typeof( People2Controller ).GetTypeInfo() );
        }

        protected override void OnAddApiVersioning( ApiVersioningOptions options )
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader(),
                new HeaderApiVersionReader( "api-version", "x-ms-version" ) );
        }

        protected override void OnConfigureServices( IServiceCollection services )
        {
            services.Replace(
                Transient( sp =>
                    new VersionedODataModelBuilder(
                        sp.GetRequiredService<IActionDescriptorCollectionProvider>(),
                        sp.GetRequiredService<IOptions<ApiVersioningOptions>>(),
                        new IModelConfiguration[]
                        {
                            new PersonModelConfiguration(),
                            new OrderModelConfiguration( supportedApiVersion: new ApiVersion( 2, 0 ) )
                        } ) ) );
            services.AddOData().EnableApiVersioning( options => options.AddModels( "api" ) );
        }
    }
}
