using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Examples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Reflection;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddApiVersioning( options => options.ReportApiVersions = true );
builder.Services.AddOData().EnableApiVersioning();
builder.Services.AddODataApiExplorer( options =>
 {
     // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
     // note: the specified format code will format the version as "'v'major[.minor][-status]"
     options.GroupNameFormat = "'v'VVV";

     // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
     // can also be used to control the format of the API version in route templates
     options.SubstituteApiVersionInUrl = true;

     // configure query options (which cannot otherwise be configured by OData conventions)
     options.QueryOptions.Controller<Microsoft.Examples.V2.PeopleController>()
                          .Action( c => c.Get( default ) )
                              .Allow( Skip | Count )
                              .AllowTop( 100 )
                              .AllowOrderBy( "firstName", "lastName" );

     options.QueryOptions.Controller<Microsoft.Examples.V3.PeopleController>()
                         .Action( c => c.Get( default ) )
                             .Allow( Skip | Count )
                             .AllowTop( 100 )
                             .AllowOrderBy( "firstName", "lastName" );
 } );
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen( options =>
 {
    // add a custom operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();

     var basePath = PlatformServices.Default.Application.ApplicationBasePath;
     var fileName = typeof( Startup ).GetTypeInfo().Assembly.GetName().Name + ".xml";

    // integrate xml comments
    options.IncludeXmlComments( Path.Combine( basePath, fileName ) );
 } );

var app = builder.Build();
var modelBuilder = app.Services.GetRequiredService<VersionedODataModelBuilder>();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseRouting();
app.UseEndpoints(
    endpoints =>
    {
        endpoints.Count();
        endpoints.MapVersionedODataRoute( "odata", "api", modelBuilder );
    } );
app.UseSwagger();
app.UseSwaggerUI(
    options =>
    {
        // build a swagger endpoint for each discovered API version
        foreach ( var description in provider.ApiVersionDescriptions )
        {
            options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
        }
    } );

app.Run();