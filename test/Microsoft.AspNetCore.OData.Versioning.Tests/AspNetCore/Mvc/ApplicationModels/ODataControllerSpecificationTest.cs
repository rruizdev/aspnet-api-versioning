namespace Microsoft.AspNetCore.Mvc.ApplicationModels
{
    using FluentAssertions;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using System;
    using System.Reflection;
    using Xunit;

    public class ODataControllerSpecificationTest
    {
        [Theory]
        [InlineData( typeof( NormalODataController ), true )]
        [InlineData( typeof( CustomODataController ), true )]
        [InlineData( typeof( NonODataController ), false )]
        public void is_satisfied_by_should_return_expected_value( Type controllerType, bool expected )
        {
            // arrange
            var specification = new ODataControllerSpecification();
            var attributes = controllerType.GetCustomAttributes( inherit: true );
            var controller = new ControllerModel( controllerType.GetTypeInfo(), attributes );

            // act
            var result = specification.IsSatisfiedBy( controller );

            // assert
            result.Should().Be( expected );
        }

        [ODataRoutePrefix( "Normal" )]
        sealed class NormalODataController : ODataController
        {
            [ODataRoute]
            public IActionResult Get() => Ok();
        }

        [ODataModel( "Custom" )]
        sealed class CustomODataController : ControllerBase
        {
            public IActionResult Get() => Ok();
        }

        [Route( "api/test" )]
        sealed class NonODataController : ControllerBase
        {
            [HttpGet]
            public IActionResult Get() => Ok();
        }
    }
}