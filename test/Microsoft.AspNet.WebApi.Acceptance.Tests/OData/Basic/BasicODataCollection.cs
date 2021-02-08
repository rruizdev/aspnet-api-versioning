namespace Microsoft.AspNet.OData.Basic
{
    using System;
    using Xunit;

    [CollectionDefinition( nameof( BasicODataCollection ) )]
    public sealed class BasicODataCollection : ICollectionFixture<BasicFixture>
    {
    }
}