namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
    using Microsoft.AspNetCore.OData;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents an <see cref="IApplicationModelProvider">application model provider</see>, which
    /// applies convention-based API versions controllers and their actions.
    /// </summary>
    [CLSCompliant( false )]
    public class ODataApplicationModelProvider : ApiVersioningApplicationModelProvider
    {
        // REF: https://github.com/OData/AspNetCoreOData/blob/master/src/Microsoft.AspNetCore.OData/Routing/ODataRoutingApplicationModelProvider.cs#L49
        const int OData = 100;
        const int BeforeOData = OData + 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataApplicationModelProvider"/> class.
        /// </summary>
        /// <param name="controllerFilter">The <see cref="IApiControllerFilter">filter</see> used for API controllers.</param>
        /// <param name="options">The current <see cref="ApiVersioningOptions">API versioning options</see>.</param>
        public ODataApplicationModelProvider( IApiControllerFilter controllerFilter, IOptions<ApiVersioningOptions> options )
            : base( options, controllerFilter ) => Order = BeforeOData;

        /// <inheritdoc />
        public override void OnProvidersExecuted( ApplicationModelProviderContext context )
        {
            base.OnProvidersExecuted( context ?? throw new ArgumentNullException( nameof( context ) ) );
            ApplyMetadataConventions( context.Result );
        }

        /// <summary>
        /// Applies metadata conventions given the provided application model.
        /// </summary>
        /// <param name="application">The <see cref="ApplicationModel">application</see> to build conventions from.</param>
        protected virtual void ApplyMetadataConventions( ApplicationModel application )
        {
            if ( application == null )
            {
                throw new ArgumentNullException( nameof( application ) );
            }

            var metadataControllers = new List<ControllerModel>();
            var supported = new HashSet<ApiVersion>();
            var deprecated = new HashSet<ApiVersion>();

            for ( var i = 0; i < application.Controllers.Count; i++ )
            {
                var controller = application.Controllers[i];

                if ( controller.ControllerType.IsMetadataController() )
                {
                    metadataControllers.Add( controller );
                    continue;
                }

                for ( var j = 0; j < controller.Actions.Count; j++ )
                {
                    var action = controller.Actions[j];
                    var model = action.GetProperty<ApiVersionModel>();

                    if ( model == null )
                    {
                        continue;
                    }

                    for ( var k = 0; k < model.SupportedApiVersions.Count; k++ )
                    {
                        supported.Add( model.SupportedApiVersions[k] );
                    }

                    for ( var k = 0; k < model.DeprecatedApiVersions.Count; k++ )
                    {
                        deprecated.Add( model.DeprecatedApiVersions[k] );
                    }
                }
            }

            var metadataController = SelectBestMetadataController( metadataControllers );

            if ( metadataController == null )
            {
                // graceful exit; in theory, this should never happen
                return;
            }

            deprecated.ExceptWith( supported );

            var conventions = Options.Conventions;
            var builder = conventions.Controller( metadataController.ControllerType )
                                     .HasApiVersions( supported )
                                     .HasDeprecatedApiVersions( deprecated );

            builder.ApplyTo( metadataController );
        }

        static ControllerModel? SelectBestMetadataController( IReadOnlyList<ControllerModel> controllers )
        {
            // note: there should be at least 2 metadata controllers, but there could be 3+
            // if a developer defines their own custom controller. ultimately, there can be
            // only one. choose and version the best controller using the following ranking:
            //
            // 1. VersionedMetadataController type (it's possible this has been removed upstream)
            // 2. original MetadataController type
            // 3. last, custom type of MetadataController from another assembly
            var bestController = default( ControllerModel );
            var original = typeof( MetadataController ).GetTypeInfo();
            var versioned = typeof( VersionedMetadataController ).GetTypeInfo();

            for ( var i = 0; i < controllers.Count; i++ )
            {
                var controller = controllers[i];

                if ( bestController == default )
                {
                    bestController = controller;
                }
                else if ( bestController.ControllerType == original &&
                          controller.ControllerType == versioned )
                {
                    bestController = controller;
                }
                else if ( bestController.ControllerType == versioned &&
                          controller.ControllerType != original )
                {
                    bestController = controller;
                }
                else if ( bestController.ControllerType != versioned &&
                          controller.ControllerType != original )
                {
                    bestController = controller;
                }
            }

            return bestController;
        }
    }
}