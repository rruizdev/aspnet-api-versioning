namespace Microsoft.AspNetCore.OData.Conventions.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Models;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Controllers;

    public class OrdersController : ODataController
    {
        public IActionResult Get( ODataQueryOptions<Order> options ) =>
            Ok( new[] { new Order() { Id = 1, Customer = "Bill Mei" } } );

        public IActionResult Get( int key, ODataQueryOptions<Order> options ) =>
            Ok( new Order() { Id = key, Customer = "Bill Mei" } );
    }
}