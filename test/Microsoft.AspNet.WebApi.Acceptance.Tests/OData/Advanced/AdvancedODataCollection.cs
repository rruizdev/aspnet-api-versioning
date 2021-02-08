namespace Microsoft.AspNet.OData.Advanced
{
    using System;
    using Xunit;

    [CollectionDefinition( nameof( AdvancedODataCollection ) )]
    public sealed class AdvancedODataCollection : ICollectionFixture<AdvancedFixture>
    {
    }
}