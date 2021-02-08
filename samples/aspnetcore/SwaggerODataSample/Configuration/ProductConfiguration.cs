namespace Microsoft.Examples.Configuration
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Examples.Models;
    using Microsoft.OData.ModelBuilder;

    /// <summary>
    /// Represents the model configuration for products.
    /// </summary>
    public class ProductConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply( ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix )
        {
            if ( apiVersion >= ApiVersions.V3 || apiVersion == ApiVersion.Neutral )
            {
                builder.EntitySet<Product>( "Products" ).EntityType.HasKey( p => p.Id );
            }
        }
    }
}