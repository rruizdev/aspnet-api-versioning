namespace Microsoft.Examples
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData;
    using Microsoft.AspNetCore.OData.Formatter;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;
    using System.Linq;

    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddControllers();
            services.AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                } );
            services.AddVersionedApiExplorer();
            services.AddOData()
                    .EnableApiVersioning(
                        options =>
                        {
                            // INFO: you do NOT and should NOT use both the query string and url segment methods together.
                            // this configuration is merely illustrating that they can coexist and allows you to easily
                            // experiment with either configuration. one of these would be removed in a real application.
                            //
                            // INFO: only pass the route prefix to GetEdmModels if you want to split the models; otherwise, both routes contain all models

                            // WHEN VERSIONING BY: query string, header, or media type
                            options.AddModels( "api" );

                            // WHEN VERSIONING BY: url segment
                            options.AddModels( "api/v{version:apiVersion}" );
                        } );
            
            services.AddMvcCore( SetOutputFormatters );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app )
        {
            app.UseRouting();
            app.UseEndpoints( endpoints => endpoints.MapControllers() );
        }

        static void SetOutputFormatters( MvcOptions options )
        {
            for ( var i = 0; i < options.InputFormatters.Count; i++ )
            {
                if ( options.InputFormatters[i] is ODataInputFormatter formatter && formatter.SupportedMediaTypes.Count == 0 )
                {
                    formatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "application/odata" ) );
                }
            }

            for ( var i = 0; i < options.OutputFormatters.Count; i++ )
            {
                if ( options.OutputFormatters[i] is ODataOutputFormatter formatter && formatter.SupportedMediaTypes.Count == 0 )
                {
                    formatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "application/odata" ) );
                }
            }
        }
    }
}