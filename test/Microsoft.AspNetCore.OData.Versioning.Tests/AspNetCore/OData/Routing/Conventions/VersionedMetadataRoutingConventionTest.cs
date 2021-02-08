namespace Microsoft.AspNetCore.OData.Routing.Conventions
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.OData.Edm;
    using System;
    using System.Reflection;
    using Xunit;
    using static System.Reflection.BindingFlags;

    public class VersionedMetadataRoutingConventionTest
    {
        [Theory]
        [InlineData( typeof( VersionedMetadataController ), true )]
        [InlineData( typeof( MetadataController ), false )]
        [InlineData( typeof( ControllerBase ), false )]
        public void applied_to_controller_should_return_expected_result( Type controllerType, bool expected )
        {
            // arrange
            var controller = new ControllerModel( controllerType.GetTypeInfo(), Array.Empty<object>() );
            var context = new ODataControllerActionContext( string.Empty, new EdmModel(), controller );
            var convention = new VersionedMetadataRoutingConvention();

            // act
            var result = convention.AppliesToController( context );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void applies_to_action_should_return_true_for_options()
        {
            // arrange
            var type = typeof( VersionedMetadataController );
            var controller = new ControllerModel( type.GetTypeInfo(), Array.Empty<object>() );
            var action = new ActionModel( type.GetRuntimeMethod( nameof( VersionedMetadataController.GetOptions ), Type.EmptyTypes ), Array.Empty<object>() );
            var context = new ODataControllerActionContext( string.Empty, new EdmModel(), controller );
            var property = context.GetType().GetProperty( nameof( ODataControllerActionContext.Action ), GetProperty | SetProperty | Instance | Public | NonPublic );

            property.SetValue( context, action );

            var convention = new VersionedMetadataRoutingConvention();

            // act
            var applies = convention.AppliesToAction( context );

            // assert
            applies.Should().BeTrue();
            action.Selectors.Should().HaveCount( 1 );
        }
    }
}