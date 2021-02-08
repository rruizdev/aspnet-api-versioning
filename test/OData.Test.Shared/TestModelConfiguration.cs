namespace Microsoft
{
#if WEBAPI
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.Web.Http;
#else
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OData.ModelBuilder;
#endif
    using System;

    public class TestModelConfiguration : IModelConfiguration
    {
        public void Apply( ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix )
        {
            var tests = builder.EntitySet<TestEntity>( "Tests" ).EntityType;
            var neutralTests = builder.EntitySet<TestNeutralEntity>( "NeutralTests" ).EntityType;

            tests.HasKey( t => t.Id );
            neutralTests.HasKey( t => t.Id );
        }
    }
}