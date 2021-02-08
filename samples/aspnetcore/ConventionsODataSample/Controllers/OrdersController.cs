namespace Microsoft.Examples.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Models;

    [ODataModel( "api/v{version:apiVersion}" )]
    public class OrdersController : ODataController
    {
        // GET ~/v1/orders
        [HttpGet]
        public IActionResult Get( ODataQueryOptions<Order> options ) =>
            Ok( new[] { new Order() { Id = 1, Customer = "Bill Mei" } } );

        // GET ~/api/v1/orders/{key}?api-version=1.0
        [HttpGet( "{key}" )]
        public IActionResult Get( int key, ODataQueryOptions<Order> options ) =>
            Ok( new Order() { Id = key, Customer = "Bill Mei" } );
    }
}