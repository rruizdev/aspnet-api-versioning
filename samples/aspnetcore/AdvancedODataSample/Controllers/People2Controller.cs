namespace Microsoft.Examples.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Examples.Models;

    [ApiVersion( "3.0" )]
    [ODataModel( "api" )]
    [ControllerName( "People" )]
    public class People2Controller : ODataController
    {
        // GET ~/api/people?api-version=3.0
        [HttpGet]
        public IActionResult Get( ODataQueryOptions<Person> options, ApiVersion version ) =>
            Ok( new[] { new Person() { Id = 1, FirstName = "Bill", LastName = "Mei", Email = "bill.mei@somewhere.com", Phone = "555-555-5555" } } );

        // GET ~/api/people/{key}?api-version=3.0
        [HttpGet( "{key}" )]
        public IActionResult Get( int key, ODataQueryOptions<Person> options, ApiVersion version ) =>
            Ok( new Person() { Id = key, FirstName = "Bill", LastName = "Mei", Email = "bill.mei@somewhere.com", Phone = "555-555-5555" } );
    }
}