namespace Microsoft.Simulators
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;

    [ApiVersion( "1.0" )]
    [ODataModel( "api" )]
    public class TestsController : ODataController
    {
        [HttpGet]
        public IActionResult Get() => Ok( new[] { new TestEntity() { Id = 1 }, new TestEntity() { Id = 2 }, new TestEntity() { Id = 3 } } );

        [HttpGet( "{key}" )]
        public IActionResult Get( int key ) => Ok( new TestEntity() { Id = key } );
    }
}