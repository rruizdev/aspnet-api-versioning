using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddApiVersioning(
    options =>
    {
        // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
        options.ReportApiVersions = true;

        // automatically applies an api version based on the name of the defining controller's namespace
        options.Conventions.Add( new VersionByNamespaceConvention() );
    } );

var app = builder.Build();

app.UseRouting();
app.UseEndpoints( builder => builder.MapControllers() );

app.Run();