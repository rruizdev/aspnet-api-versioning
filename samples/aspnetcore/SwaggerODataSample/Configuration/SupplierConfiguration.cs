namespace Microsoft.Examples.Configuration
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Examples.Models;
    using Microsoft.OData.ModelBuilder;

    /// <summary>
    /// Represents the model configuration for suppliers.
    /// </summary>
    public class SupplierConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply( ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix )
        {
            if ( apiVersion >= ApiVersions.V3 || apiVersion == ApiVersion.Neutral )
            {
                builder.EntitySet<Supplier>( "Suppliers" ).EntityType.HasKey( p => p.Id );
                builder.Singleton<Supplier>( "Acme" );
            }
        }
    }
}