using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Examples.Controllers;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddApiVersioning(
    options =>
    {
        // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
        options.ReportApiVersions = true;

        options.Conventions.Controller<ValuesController>().HasApiVersion( 1, 0 );

        options.Conventions.Controller<Values2Controller>()
            .HasApiVersion( 2, 0 )
            .HasApiVersion( 3, 0 )
            .Action( c => c.GetV3( default ) ).MapToApiVersion( 3, 0 )
            .Action( c => c.GetV3( default, default ) ).MapToApiVersion( 3, 0 );

        options.Conventions.Controller<HelloWorldController>()
            .HasApiVersion( 1, 0 )
            .HasApiVersion( 2, 0 )
            .AdvertisesApiVersion( 3, 0 );
    } );

var app = builder.Build();

app.UseRouting();
app.UseEndpoints( builder => builder.MapControllers() );

app.Run();