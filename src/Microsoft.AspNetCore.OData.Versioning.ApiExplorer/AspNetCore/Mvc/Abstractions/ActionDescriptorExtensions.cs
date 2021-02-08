namespace Microsoft.AspNetCore.Mvc.Abstractions
{
    using Microsoft.AspNetCore.OData.Routing;
    using System;

    static class ActionDescriptorExtensions
    {
        internal static ODataRoutingMetadata? GetODataRoute( this ActionDescriptor action )
        {
            var metadata = action.EndpointMetadata;

            for ( var i = 0; i < metadata.Count; i++ )
            {
                if ( metadata[i] is ODataRoutingMetadata odataRoute )
                {
                    return odataRoute;
                }
            }

            return default;
        }
    }
}