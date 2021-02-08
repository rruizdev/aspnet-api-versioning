namespace Microsoft.Examples.V3
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Examples.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Represents a RESTful service for the ACME supplier.
    /// </summary>
    [ApiVersion( "3.0" )]
    [ODataModel( "api" )]
    public class AcmeController : ODataController
    {
        /// <summary>
        /// Retrieves the ACME supplier.
        /// </summary>
        /// <returns>All available suppliers.</returns>
        /// <response code="200">The supplier successfully retrieved.</response>
        [HttpGet]
        [EnableQuery]
        [Produces( "application/json" )]
        [ProducesResponseType( typeof( ODataValue<Supplier> ), Status200OK )]
        public IActionResult Get() => Ok( NewSupplier() );

        /// <summary>
        /// Gets the products associated with the supplier.
        /// </summary>
        /// <returns>The associated supplier products.</returns>
        [HttpGet( "Products" )]
        [EnableQuery]
        public IQueryable<Product> GetProducts() => NewSupplier().Products.AsQueryable();

        /// <summary>
        /// Links a product to a supplier.
        /// </summary>
        /// <param name="navigationProperty">The product to link.</param>
        /// <param name="link">The product identifier.</param>
        /// <returns>None</returns>
        [HttpPost( "{navigationProperty}/$ref" )]
        [ProducesResponseType( Status204NoContent )]
        [ProducesResponseType( Status404NotFound )]
        public IActionResult CreateRef( string navigationProperty, [FromBody] Uri link ) => NoContent();

        /// <summary>
        /// Unlinks a product from a supplier.
        /// </summary>
        /// <param name="navigationProperty">The product to unlink.</param>
        /// <param name="relatedKey">The related product identifier.</param>
        /// <returns>None</returns>
        [HttpDelete( "{navigationProperty}/$ref" )]
        [ProducesResponseType( Status204NoContent )]
        [ProducesResponseType( Status404NotFound )]
        public IActionResult DeleteRef( string navigationProperty, [FromQuery( Name = "$id" )] string relatedKey ) => NoContent();

        private static Supplier NewSupplier() =>
            new Supplier()
            {
                Id = 42,
                Name = "Acme",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Id = 42,
                        Name = "Product 42",
                        Category = "Test",
                        Price = 42,
                        SupplierId = 42,
                    }
                },
            };
    }
}
