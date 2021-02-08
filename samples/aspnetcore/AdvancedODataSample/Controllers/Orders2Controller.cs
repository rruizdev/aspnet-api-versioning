namespace Microsoft.Examples.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Examples.Models;

    [ApiVersion( "2.0" )]
    [ODataModel( "api" )]
    [ControllerName( "Orders" )]
    public class Orders2Controller : ODataController
    {
        // GET ~/api/orders?api-version=2.0
        [HttpGet]
        public IActionResult Get( ODataQueryOptions<Order> options, ApiVersion version ) =>
            Ok( new[] { new Order() { Id = 1, Customer = $"Customer v{version}" } } );

        // GET ~/api/orders/{key}?api-version=2.0
        [HttpGet( "{key}" )]
        public IActionResult Get( int key, ODataQueryOptions<Order> options, ApiVersion version ) =>
            Ok( new Order() { Id = key, Customer = $"Customer v{version}" } );
    }
}