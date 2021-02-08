#if WEBAPI
namespace Microsoft.AspNet.OData
#else
namespace Microsoft.AspNetCore.OData
#endif
{
#if WEBAPI
    using Microsoft.AspNet.OData.Query;
    using Microsoft.AspNet.OData.Routing;
#else
    using Microsoft.AspNetCore.OData.Formatter;
    using Microsoft.AspNetCore.OData.Formatter.Value;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Microsoft.OData.UriParser;
#endif
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    static partial class TypeExtensions
    {
#if WEBAPI
        static readonly Type ODataRoutingAttributeType = typeof( ODataRoutingAttribute );
#else
        static readonly TypeInfo ODataController = typeof( ODataController ).GetTypeInfo();
        static readonly Type ODataModelAttributeType = typeof( ODataModelAttribute );
#endif
        static readonly TypeInfo MetadataController = typeof( MetadataController ).GetTypeInfo();
        static readonly Type Delta = typeof( IDelta );
        static readonly Type ODataPath = typeof( ODataPath );
        static readonly Type ODataQueryOptions = typeof( ODataQueryOptions );
        static readonly Type ODataActionParameters = typeof( ODataActionParameters );
#if WEBAPI
        static readonly Type ODataParameterHelper = typeof( ODataParameterHelper );

        internal static bool IsODataController( this Type controllerType ) => Attribute.IsDefined( controllerType, ODataRoutingAttributeType );

        internal static bool IsODataController( this TypeInfo controllerType ) => Attribute.IsDefined( controllerType, ODataRoutingAttributeType );
#else
        internal static bool IsODataController( this Type controllerType ) => controllerType.GetTypeInfo().IsODataController();

        internal static bool IsODataController( this TypeInfo controllerType ) =>
            ODataController.IsAssignableFrom( controllerType ) || Attribute.IsDefined( controllerType, ODataModelAttributeType );
#endif
        internal static bool IsMetadataController( this TypeInfo controllerType ) => MetadataController.IsAssignableFrom( controllerType );

        internal static bool IsODataPath( this Type type ) => ODataPath.IsAssignableFrom( type );

        internal static bool IsODataQueryOptions( this Type type ) => ODataQueryOptions.IsAssignableFrom( type );

        internal static bool IsODataActionParameters( this Type type ) => ODataActionParameters.IsAssignableFrom( type );

        internal static bool IsDelta( this Type type ) => Delta.IsAssignableFrom( type );

        internal static bool IsModelBound( this Type type ) =>
           ODataPath.IsAssignableFrom( type )
           || ODataQueryOptions.IsAssignableFrom( type )
           || Delta.IsAssignableFrom( type )
           || ODataActionParameters.IsAssignableFrom( type )
#if WEBAPI
           || ODataParameterHelper.Equals( type );
#else
            ;
#endif
    }
}