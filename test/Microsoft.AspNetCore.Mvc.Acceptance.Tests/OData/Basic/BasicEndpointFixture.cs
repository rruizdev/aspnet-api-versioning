namespace Microsoft.AspNetCore.OData.Basic
{
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.OData.Basic.Controllers.Endpoint;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public class BasicEndpointFixture : ODataFixture
    {
        public BasicEndpointFixture()
        {
            EnableEndpointRouting = true;
            FilteredControllerTypes.Add( typeof( OrdersController ).GetTypeInfo() );
            FilteredControllerTypes.Add( typeof( PeopleController ).GetTypeInfo() );
            FilteredControllerTypes.Add( typeof( People2Controller ).GetTypeInfo() );
            FilteredControllerTypes.Add( typeof( CustomersController ).GetTypeInfo() );
        }

        protected override void OnAddApiVersioning( ApiVersioningOptions options ) => options.ReportApiVersions = true;

        protected override void OnConfigureServices( IServiceCollection services ) =>
            services.AddOData()
                    .EnableApiVersioning(
                        options => options.AddModels( "api" )
                                          .AddModels( "v{version:apiVersion}" ) );
    }
}
