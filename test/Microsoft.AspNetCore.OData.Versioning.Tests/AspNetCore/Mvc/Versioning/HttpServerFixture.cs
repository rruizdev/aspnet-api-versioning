namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.OData;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;

    public class HttpServerFixture : IDisposable
    {
        readonly TestServer server;
        bool disposed;

        ~HttpServerFixture() => Dispose( false );

        public HttpServerFixture()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(
                    services =>
                    {
                        services.AddControllers();
                        services.AddApiVersioning();
                        services.AddOData().EnableApiVersioning( options => options.AddModels( "api" ) );
                    } )
                .Configure(
                    app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints( endpoints => endpoints.MapControllers() );
                    } );

            server = new TestServer( builder );
            Client = server.CreateClient();
            Client.BaseAddress = new Uri( "http://localhost" );
        }

        public HttpClient Client { get; }

        public IServiceProvider Services => server.Host.Services;

        void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( !disposing )
            {
                return;
            }

            Client.Dispose();
            server.Dispose();
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}