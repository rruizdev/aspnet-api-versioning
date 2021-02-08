namespace Microsoft.AspNet.OData
{
#if WEBAPI
    using Newtonsoft.Json;
#endif
    using System;
#if !WEBAPI
    using JsonProperty = System.Text.Json.Serialization.JsonPropertyNameAttribute;
#endif

    /// <summary>
    /// Represents an OData identifier specified in the body of a POST or PUT OData relationship reference request.
    /// </summary>
    public class ODataId
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The <see cref="Uri">URL</see> representing the related entity identifier.</value>
        [JsonProperty( "@odata.id" )]
        public Uri Value { get; set; } = default!;
    }
}