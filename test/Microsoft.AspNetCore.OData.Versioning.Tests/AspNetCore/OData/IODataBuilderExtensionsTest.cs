namespace Microsoft.AspNetCore.OData
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
    using Microsoft.AspNetCore.OData.Abstracts;
    using Microsoft.AspNetCore.OData.Routing.Conventions;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OData.ModelBuilder;
    using Moq;
    using System;
    using System.Linq;
    using Xunit;

    public class IODataBuilderExtensionsTest
    {
        [Fact]
        public void enable_api_versioning_should_register_expected_services()
        {
            // arrange
            var services = new ServiceCollection();
            var mock = new Mock<IODataBuilder>();

            mock.SetupGet( b => b.Services ).Returns( services );

            var builder = mock.Object;

            // act
            builder.EnableApiVersioning();

            // assert
            services.Any( sd => sd.ServiceType == typeof( IHttpContextAccessor ) ).Should().BeTrue();
            services.Single( sd => sd.ServiceType == typeof( ODataRouteConfiguration ) ).ImplementationType.Should().Be( typeof( ODataRouteConfiguration ) );
            services.Single( sd => sd.ServiceType == typeof( VersionedODataModelBuilder ) ).ImplementationType.Should().Be( typeof( VersionedODataModelBuilder ) );
            services.Any( sd => sd.ServiceType == typeof( Func<ODataModelBuilder> ) ).Should().BeTrue();
            services.Any( sd => sd.ServiceType == typeof( IApiControllerSpecification ) && sd.ImplementationType == typeof( ODataControllerSpecification ) ).Should().BeTrue();
            services.Any( sd => sd.ServiceType == typeof( IConfigureOptions<ODataOptions> ) && sd.ImplementationType == typeof( ODataOptionsSetup ) ).Should().BeTrue();
            services.Any( sd => sd.ServiceType == typeof( MatcherPolicy ) && sd.ImplementationType == typeof( DefaultMetadataMatcherPolicy ) ).Should().BeTrue();
            services.Any( sd => sd.ServiceType == typeof( IApplicationModelProvider ) && sd.ImplementationType == typeof( ODataApplicationModelProvider ) ).Should().BeTrue();
            services.Any( sd => sd.ServiceType == typeof( IApplicationModelProvider ) && sd.ImplementationType == typeof( ApiVersioningApplicationModelProvider ) ).Should().BeFalse();
            services.Any( sd => sd.ServiceType == typeof( IODataControllerActionConvention ) && sd.ImplementationType == typeof( VersionedMetadataRoutingConvention ) ).Should().BeTrue();
            services.Any( sd => sd.ServiceType == typeof( IODataControllerActionConvention ) && sd.ImplementationType == typeof( MetadataRoutingConvention ) ).Should().BeFalse();
            services.Any( sd => sd.ServiceType == typeof( IApplicationModelProvider ) && sd.ImplementationType.Name == "ODataRouteConfigurationScope" ).Should().BeTrue();
        }
    }
}