namespace Microsoft.Examples.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Formatter.Value;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Examples.Models;

    // note: since the application is configured with AssumeDefaultVersionWhenUnspecified, this controller
    // is resolved without or without an API version, even though it is explicitly versioned
    [ApiVersion( "1.0" )]
    [ApiVersion( "2.0" )]
    [ODataModel( "api" )]
    public class PeopleController : ODataController
    {
        // GET ~/api/people
        // GET ~/api/people?api-version=[1.0|2.0]
        [HttpGet]
        public IActionResult Get( ODataQueryOptions<Person> options, ApiVersion version ) =>
            Ok( new[] { new Person() { Id = 1, FirstName = "Bill", LastName = "Mei", Email = "bill.mei@somewhere.com", Phone = "555-555-5555" } } );

        // GET ~/api/people/{key}
        // GET ~/api/people/{key}?api-version=[1.0|2.0]
        [HttpGet( "{key}" )]
        public IActionResult Get( int key, ODataQueryOptions<Person> options, ApiVersion version ) =>
            Ok( new Person() { Id = key, FirstName = "Bill", LastName = "Mei", Email = "bill.mei@somewhere.com", Phone = "555-555-5555" } );

        // PATCH ~/api/people/{key}?api-version=2.0
        [HttpPatch( "{key}" )]
        [MapToApiVersion( "2.0" )]
        public IActionResult Patch( int key, Delta<Person> delta, ODataQueryOptions<Person> options, ApiVersion version )
        {
            if ( !ModelState.IsValid )
                return BadRequest( ModelState );

            var person = new Person() { Id = key, FirstName = "Bill", LastName = "Mei", Email = "bill.mei@somewhere.com", Phone = "555-555-5555" };

            delta.Patch( person );

            return Updated( person );
        }
    }
}