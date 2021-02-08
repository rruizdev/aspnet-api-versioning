#if WEBAPI
namespace Microsoft.AspNet.OData.Builder
#else
namespace Microsoft.AspNetCore.OData.Query
#endif
{
#if WEBAPI
    using Microsoft.AspNet.OData.Query;
    using System.Web.Http.Description;
    using ControllerActionDescriptor = System.Web.Http.Controllers.ReflectedHttpActionDescriptor;
#else
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.OData.Query.Validator;
#endif

    sealed class ODataControllerQueryOptionConvention : IODataQueryOptionsConvention
    {
        readonly ODataActionQueryOptionConventionLookup lookup;
        readonly ODataQueryOptionSettings settings;

        internal ODataControllerQueryOptionConvention(
            ODataActionQueryOptionConventionLookup lookup,
            ODataQueryOptionSettings settings )
        {
            this.lookup = lookup;
            this.settings = settings;
        }

        public void ApplyTo( ApiDescription apiDescription )
        {
            if ( apiDescription.ActionDescriptor is not ControllerActionDescriptor action )
            {
                return;
            }

            if ( !lookup( action.MethodInfo, settings, out var convention ) )
            {
                convention = ImplicitActionConvention( settings );
            }

            convention!.ApplyTo( apiDescription );
        }

        static IODataQueryOptionsConvention ImplicitActionConvention( ODataQueryOptionSettings settings )
        {
            var validationSettings = new ODataValidationSettings()
            {
                AllowedArithmeticOperators = AllowedArithmeticOperators.None,
                AllowedFunctions = AllowedFunctions.None,
                AllowedLogicalOperators = AllowedLogicalOperators.None,
                AllowedQueryOptions = AllowedQueryOptions.None,
            };

            return new ODataValidationSettingsConvention( validationSettings, settings );
        }
    }
}