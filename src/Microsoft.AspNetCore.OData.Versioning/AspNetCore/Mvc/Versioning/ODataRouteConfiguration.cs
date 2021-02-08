namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using System;
    using static ODataRouteConfigurationState;

    /// <summary>
    /// Represents the OData route configuration.
    /// </summary>
    public class ODataRouteConfiguration
    {
        /// <summary>
        /// Gets the OData route configuration state.
        /// </summary>
        /// <value>One of the <see cref="ODataRouteConfigurationState"/> values.</value>
        public virtual ODataRouteConfigurationState State { get; internal set; } = Unconfigured;
    }
}