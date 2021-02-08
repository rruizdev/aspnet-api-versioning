namespace Microsoft.AspNetCore.OData.Routing.Conventions
{
    using Microsoft.AspNetCore.OData.Extensions;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.AspNetCore.OData.Routing.Template;
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents the <see cref="IODataControllerActionConvention">OData routing convention</see> for versioned service and metadata documents.
    /// </summary>
    [CLSCompliant( false )]
    public class VersionedMetadataRoutingConvention : MetadataRoutingConvention
    {
        static readonly Type metadataController = typeof( VersionedMetadataController ).GetTypeInfo();

        /// <inheritdoc />
        public override bool AppliesToController( ODataControllerActionContext context )
        {
            if ( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }

            return metadataController.IsAssignableFrom( context.Controller.ControllerType );
        }

        /// <inheritdoc />
        public override bool AppliesToAction( ODataControllerActionContext context )
        {
            if ( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }

            var action = context.Action;
            var actionName = action.ActionMethod.Name;

            if ( actionName == nameof( VersionedMetadataController.GetOptions ) )
            {
                var template = new ODataPathTemplate( MetadataSegmentTemplate.Instance );
                action.AddSelector( "Options", context.Prefix, context.Model, template );
                return true;
            }

            return base.AppliesToAction( context );
        }
    }
}