#pragma warning disable CA1812

namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using static ODataRouteConfigurationState;

    sealed class ODataRouteConfigurationScope : IApplicationModelProvider
    {
        const int Last = int.MinValue;
        readonly ODataRouteConfiguration configuration;

        public ODataRouteConfigurationScope( ODataRouteConfiguration configuration ) => this.configuration = configuration;

        public int Order => Last;

        public void OnProvidersExecuting( ApplicationModelProviderContext context ) => configuration.State = Configuring;

        public void OnProvidersExecuted( ApplicationModelProviderContext context ) => configuration.State = Configured;
    }
}