namespace Microsoft.Extensions.DependencyInjection
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.OData;
    using System.Linq;
    using Xunit;

    public class IServiceCollectionExtensionsTest
    {
        [Fact]
        public void add_odata_api_explorer_should_configure_mvc()
        {
            // arrange
            var services = new ServiceCollection();

            // act
            services.AddODataApiExplorer();

            // assert
            services.Single( sd => sd.ServiceType == typeof( IModelTypeBuilder ) ).ImplementationType.Should().Be( typeof( DefaultModelTypeBuilder ) );
            services.Single( sd => sd.ServiceType == typeof( IApiVersionDescriptionProvider ) ).ImplementationType.Should().Be( typeof( DefaultApiVersionDescriptionProvider ) );
            services.Single( sd => sd.ImplementationType == typeof( VersionedApiDescriptionProvider ) ).ServiceType.Should().Be( typeof( IApiDescriptionProvider ) );
            services.Single( sd => sd.ImplementationType == typeof( ODataApiDescriptionProvider ) ).ServiceType.Should().Be( typeof( IApiDescriptionProvider ) );
        }
    }
}