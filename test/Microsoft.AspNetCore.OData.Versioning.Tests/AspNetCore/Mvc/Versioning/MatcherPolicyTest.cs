namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using FluentAssertions;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;
    using static System.Net.HttpStatusCode;

    public class MatcherPolicyTest : IClassFixture<HttpServerFixture>
    {
        [Theory]
        [InlineData( "1.0" )]
        [InlineData( "2.0" )]
        [InlineData( "3.0" )]
        public async Task apply_async_should_match_versioned_endpoint( string version )
        {
            // arrange
            var requestUri = $"api/tests?api-version={version}";

            // act
            var response = await Client.GetAsync( requestUri );

            // assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Theory]
        [InlineData( "" )]
        [InlineData( "?api-version=2.0" )]
        public async Task apply_async_should_match_versionX2Dneutral_endpoint( string queryString )
        {
            // arrange
            var requestUri = "api/neutraltests" + queryString;

            // act
            var response = await Client.GetAsync( requestUri );

            // assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Theory]
        [InlineData( "" )]
        [InlineData( "$metadata" )]
        [InlineData( "$metadata?api-version=2.0" )]
        public async Task apply_async_should_match_metadata_endpoint( string relativeUrl )
        {
            // arrange
            var requestUri = "api/" + relativeUrl;

            // act
            var response = await Client.GetAsync( requestUri );

            // assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Theory]
        [InlineData( "$metadata" )]
        [InlineData( "tests" )]
        public async Task apply_async_should_return_400_for_unsupported_version( string path )
        {
            // arrange
            var requestUri = $"api/{path}?api-version=4.0";

            // act
            var response = await Client.GetAsync( requestUri );

            // assert
            response.StatusCode.Should().Be( BadRequest );
        }

        [Fact]
        public async Task apply_async_should_return_404_for_nonexistent_endpoint()
        {
            // arrange
            var requestUri = "api/fake?api-version=1.0";

            // act
            var response = await Client.GetAsync( requestUri );

            // assert
            response.StatusCode.Should().Be( NotFound );
        }

        public MatcherPolicyTest( HttpServerFixture fixture ) => Client = fixture.Client;

        HttpClient Client { get; }
    }
}