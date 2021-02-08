namespace Microsoft.AspNetCore.Mvc.Routing
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.OData.Routing;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Matching;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static Microsoft.AspNetCore.Mvc.Versioning.ApiVersionMapping;
    using static Microsoft.AspNetCore.OData.Routing.Template.ODataSegmentKind;
    using static System.Threading.Tasks.Task;

    /// <summary>
    /// Represents the <see cref="IEndpointSelectorPolicy">endpoint selector policy</see> for the default
    /// service document and $metadata endpoint.
    /// </summary>
    [CLSCompliant( false )]
    public class DefaultMetadataMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        const int BeforeApiVersioning = -100;

        /// <inheritdoc />
        public override int Order => BeforeApiVersioning;

        /// <inheritdoc />
        public virtual bool AppliesToEndpoints( IReadOnlyList<Endpoint> endpoints )
        {
            if ( endpoints == null )
            {
                throw new ArgumentNullException( nameof( endpoints ) );
            }

            for ( var i = 0; i < endpoints.Count; i++ )
            {
                if ( IsServiceDocumentOrMetadataEndpoint( endpoints[i].Metadata ) )
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public virtual Task ApplyAsync( HttpContext httpContext, CandidateSet candidates )
        {
            if ( httpContext == null )
            {
                throw new ArgumentNullException( nameof( httpContext ) );
            }

            if ( candidates == null )
            {
                throw new ArgumentNullException( nameof( candidates ) );
            }

            var feature = httpContext.Features.Get<IApiVersioningFeature>();

            try
            {
                if ( !string.IsNullOrEmpty( feature.RawRequestedApiVersion ) )
                {
                    return CompletedTask;
                }
            }
            catch ( AmbiguousApiVersionException )
            {
                return CompletedTask;
            }

            for ( var i = 0; i < candidates.Count; i++ )
            {
                if ( !candidates.IsValidCandidate( i ) )
                {
                    continue;
                }

                ref var candidate = ref candidates[i];
                var metadata = candidate.Endpoint.Metadata;

                if ( !IsServiceDocumentOrMetadataEndpoint( metadata ) )
                {
                    continue;
                }

                var model = metadata.GetMetadata<ActionDescriptor>()?.GetApiVersionModel( Explicit | Implicit );

                if ( model != null && model.DeclaredApiVersions.Count > 0 )
                {
                    feature.RequestedApiVersion = model.DeclaredApiVersions[0];
                }
            }

            return CompletedTask;
        }

        static bool IsServiceDocumentOrMetadataEndpoint( EndpointMetadataCollection metadata )
        {
            var odata = metadata.GetMetadata<ODataRoutingMetadata>();

            if ( odata == null )
            {
                return false;
            }

            var segments = odata.Template.Segments;

            return segments.Count == 0 || ( segments.Count == 1 && segments[0].Kind == Metadata );
        }
    }
}