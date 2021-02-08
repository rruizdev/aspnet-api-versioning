namespace Microsoft.AspNetCore.OData.Simulators.Configuration
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Simulators.Models;
    using Microsoft.OData.ModelBuilder;

    /// <summary>
    /// Represents the model configuration for products.
    /// </summary>
    public class ProductConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply( ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix )
        {
            if ( apiVersion != ApiVersion.Neutral && apiVersion < ApiVersions.V3 )
            {
                return;
            }

            var product = builder.EntitySet<Product>( "Products" ).EntityType.HasKey( p => p.Id );
        }
    }
}