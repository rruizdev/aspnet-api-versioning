namespace Microsoft.AspNetCore.OData.Query
{
#if WEBAPI
    using Microsoft.AspNet.OData.Query;
#else
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.OData.Query;
#endif
    using System;

    /// <content>
    /// Provides additional implementation specific to Microsoft ASP.NET Core.
    /// </content>
    [CLSCompliant( false )]
    public partial class ODataQueryOptionSettings
    {
        /// <summary>
        /// Gets or sets the default OData query settings.
        /// </summary>
        /// <value>The <see cref="DefaultQuerySettings">default OData query settings</see>.</value>
        public DefaultQuerySettings? DefaultQuerySettings { get; set; }

        /// <summary>
        /// Gets or sets the configured model metadata provider.
        /// </summary>
        /// <value>The configured <see cref="IModelMetadataProvider">model metadata provider</see>.</value>
        public IModelMetadataProvider? ModelMetadataProvider { get; set; }
    }
}