namespace Microsoft.AspNetCore.OData
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.OData.Configuration;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public abstract class ODataFixture : HttpServerFixture
    {
        protected ODataFixture() => FilteredControllerTypes.Add( typeof( VersionedMetadataController ).GetTypeInfo() );

        protected override void OnConfigurePartManager( ApplicationPartManager partManager )
        {
            base.OnConfigurePartManager( partManager );

            partManager.ApplicationParts.Add(
                new TestApplicationPart(
                    typeof( OrderModelConfiguration ),
                    typeof( PersonModelConfiguration ),
                    typeof( CustomerModelConfiguration ) ) );
        }

        protected override void OnConfigureServices( IServiceCollection services ) => services.AddOData().EnableApiVersioning();
    }
}