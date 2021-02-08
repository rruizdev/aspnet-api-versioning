namespace Microsoft
{
#if WEBAPI
    using Microsoft.AspNet.OData.Builder;
#endif
    using Microsoft.OData.Edm;
#if !WEBAPI
    using Microsoft.OData.ModelBuilder;
#endif

    internal static class Test
    {
        static Test()
        {
            var builder = new ODataModelBuilder();
            var tests = builder.EntitySet<TestEntity>( "Tests" ).EntityType;
            var neutralTests = builder.EntitySet<TestNeutralEntity>( "NeutralTests" ).EntityType;

            tests.HasKey( t => t.Id );
            neutralTests.HasKey( t => t.Id );
            Model = builder.GetEdmModel();
        }

        internal static IEdmModel Model { get; }

        internal static IEdmModel EmptyModel { get; } = new ODataModelBuilder().GetEdmModel();
    }
}