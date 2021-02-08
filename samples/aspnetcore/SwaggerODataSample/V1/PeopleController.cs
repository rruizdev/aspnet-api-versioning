namespace Microsoft.Examples.V1
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Examples.Models;
    using System.Linq;
    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Represents a RESTful people service.
    /// </summary>
    [ApiVersion( "1.0" )]
    [ApiVersion( "0.9", Deprecated = true )]
    [ODataModel( "api" )]
    public class PeopleController : ODataController
    {
        /// <summary>
        /// Gets a single person.
        /// </summary>
        /// <param name="key">The requested person identifier.</param>
        /// <param name="options">The current OData query options.</param>
        /// <returns>The requested person.</returns>
        /// <response code="200">The person was successfully retrieved.</response>
        /// <response code="404">The person does not exist.</response>
        [HttpGet( "{key}" )]
        [Produces( "application/json" )]
        [ProducesResponseType( typeof( Person ), Status200OK )]
        [ProducesResponseType( Status404NotFound )]
        public IActionResult Get( int key, ODataQueryOptions<Person> options )
        {
            var people = new[]
            {
                new Person()
                {
                    Id = key,
                    FirstName = "John",
                    LastName = "Doe",
                }
            };

            var person = options.ApplyTo( people.AsQueryable() ).SingleOrDefault();

            if ( person == null )
            {
                return NotFound();
            }

            return Ok( person );
        }
    }
}