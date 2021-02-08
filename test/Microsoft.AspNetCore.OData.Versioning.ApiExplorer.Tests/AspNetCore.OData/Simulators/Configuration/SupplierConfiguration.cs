namespace Microsoft.AspNetCore.OData.Simulators.Configuration
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Simulators.Models;
    using Microsoft.OData.ModelBuilder;

    /// <summary>
    /// Represents the model configuration for suppliers.
    /// </summary>
    public class SupplierConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply( ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix )
        {
            if ( apiVersion != ApiVersion.Neutral && apiVersion < ApiVersions.V3 )
            {
                return;
            }

            var supplier = builder.EntitySet<Supplier>( "Suppliers" ).EntityType.HasKey( p => p.Id );
        }
    }
}