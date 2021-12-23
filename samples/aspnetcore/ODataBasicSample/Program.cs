using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddApiVersioning(
    options =>
    {
        // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
        options.ReportApiVersions = true;
    } );
builder.Services.AddOData().EnableApiVersioning();

var app = builder.Build();
var modelBuilder = app.Services.GetRequiredService<VersionedODataModelBuilder>();

app.UseRouting();
app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapControllers();

        // INFO: you do NOT and should NOT use both the query string and url segment methods together.
        // this configuration is merely illustrating that they can coexist and allows you to easily
        // experiment with either configuration. one of these would be removed in a real application.
        //
        // INFO: only pass the route prefix to GetEdmModels if you want to split the models; otherwise, both routes contain all models

        // WHEN VERSIONING BY: query string, header, or media type
        endpoints.MapVersionedODataRoute( "odata", "api", modelBuilder );

        // WHEN VERSIONING BY: url segment
        endpoints.MapVersionedODataRoute( "odata-bypath", "api/v{version:apiVersion}", modelBuilder );
    } );

app.Run();