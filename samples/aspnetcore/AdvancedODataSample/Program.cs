using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiVersioning(
    options =>
    {
        // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
        options.ReportApiVersions = true;

        // allows a client to make a request without specifying an api version. the value of
        // options.DefaultApiVersion will be 'assumed'; this is meant to grandfather in legacy apis
        options.AssumeDefaultVersionWhenUnspecified = true;

        // allow multiple locations to request an api version
        options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader(),
            new HeaderApiVersionReader( "api-version", "x-ms-version" ) );
    } );
builder.Services.AddOData().EnableApiVersioning();

var app = builder.Build();
var modelBuilder = app.Services.GetRequiredService<VersionedODataModelBuilder>();

app.UseRouting();
app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapVersionedODataRoute( "odata", "api", modelBuilder.GetEdmModels() );
    } );

app.Run();