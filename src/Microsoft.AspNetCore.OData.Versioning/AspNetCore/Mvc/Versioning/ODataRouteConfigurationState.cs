namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using System;

    /// <summary>
    /// Represents the possible OData route configuration states.
    /// </summary>
    public enum ODataRouteConfigurationState
    {
        /// <summary>
        /// Indicates the OData routes have not been configured.
        /// </summary>
        Unconfigured,

        /// <summary>
        /// Indicates the OData routes are being configured.
        /// </summary>
        Configuring,

        /// <summary>
        /// Indicates the OData routes have been configured.
        /// </summary>
        Configured,
    }
}