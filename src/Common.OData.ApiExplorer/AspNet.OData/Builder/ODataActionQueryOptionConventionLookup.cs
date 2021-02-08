#if WEBAPI
namespace Microsoft.AspNet.OData.Builder
#else
namespace Microsoft.AspNetCore.OData.Query
#endif
{
    using System;
    using System.Reflection;

    delegate bool ODataActionQueryOptionConventionLookup( MethodInfo action, ODataQueryOptionSettings settings, out IODataQueryOptionsConvention? convention );
}