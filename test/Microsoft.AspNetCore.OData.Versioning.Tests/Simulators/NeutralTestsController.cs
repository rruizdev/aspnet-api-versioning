namespace Microsoft.Simulators
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;

    [ApiVersionNeutral]
    [ODataModel( "api" )]
    public class NeutralTestsController : ODataController
    {
        [HttpGet]
        public IActionResult Get() => Ok( new[] { new TestNeutralEntity() { Id = 1 }, new TestNeutralEntity() { Id = 2 }, new TestNeutralEntity() { Id = 3 } } );

        [HttpGet( "{key}" )]
        public IActionResult Get( int key ) => Ok( new TestNeutralEntity() { Id = key } );
    }
}
