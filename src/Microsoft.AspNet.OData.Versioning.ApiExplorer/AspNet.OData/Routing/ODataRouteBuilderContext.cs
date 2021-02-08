namespace Microsoft.AspNet.OData.Routing
{
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Routing.Conventions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;
    using Microsoft.Web.Http;
    using Microsoft.Web.Http.Description;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Description;
    using static Microsoft.OData.ODataUrlKeyDelimiter;
    using static ODataRouteTemplateGenerationKind;
    using static System.Linq.Enumerable;
    using static System.StringComparison;

    sealed class ODataRouteBuilderContext
    {
        readonly ODataRouteAttribute? routeAttribute;
        readonly ODataRoute route;
        IODataPathTemplateHandler? templateHandler;

        internal ODataRouteBuilderContext(
            HttpConfiguration configuration,
            ApiVersion apiVersion,
            ODataRoute route,
            HttpActionDescriptor actionDescriptor,
            IList<ApiParameterDescription> parameterDescriptions,
            IModelTypeBuilder modelTypeBuilder,
            ODataApiExplorerOptions options )
        {
            this.route = route;
            ApiVersion = apiVersion;
            Services = configuration.GetODataRootContainer( route );
            routeAttribute = actionDescriptor.GetCustomAttributes<ODataRouteAttribute>().FirstOrDefault();
            RouteTemplate = routeAttribute?.PathTemplate;
            RoutePrefix = route.RoutePrefix?.Trim( '/' );
            ActionDescriptor = actionDescriptor;
            ParameterDescriptions = parameterDescriptions;
            Options = options;
            UrlKeyDelimiter = UrlKeyDelimiterOrDefault( configuration.GetUrlKeyDelimiter() ?? Services.GetService<IODataPathHandler>()?.UrlKeyDelimiter );

            var selector = Services.GetRequiredService<IEdmModelSelector>();
            var model = selector.SelectModel( apiVersion );
            var container = model?.EntityContainer;

            if ( model == null || container == null )
            {
                EdmModel = Services.GetRequiredService<IEdmModel>();
                IsRouteExcluded = true;
                return;
            }

            var controllerName = actionDescriptor.ControllerDescriptor.ControllerName;

            EdmModel = model;
            Services = new FixedEdmModelServiceProviderDecorator( Services, model );
            EntitySet = container.FindEntitySet( controllerName );
            Singleton = container.FindSingleton( controllerName );
            Operation = ResolveOperation( container, actionDescriptor.ActionName );
            ActionType = GetActionType( actionDescriptor );
            IsRouteExcluded = ActionType == ODataRouteActionType.Unknown;

            if ( Operation?.IsAction() == true )
            {
                ConvertODataActionParametersToTypedModel( modelTypeBuilder, (IEdmAction) Operation, controllerName );
            }
        }

        internal IServiceProvider Services { get; }

        internal ApiVersion ApiVersion { get; }

        internal ODataApiExplorerOptions Options { get; }

        internal IList<ApiParameterDescription> ParameterDescriptions { get; }

        internal ODataRouteTemplateGenerationKind RouteTemplateGeneration { get; } = Client;

        internal IEdmModel EdmModel { get; }

        internal string? RouteTemplate { get; }

        internal string? RoutePrefix { get; }

        internal HttpActionDescriptor ActionDescriptor { get; }

        internal IEdmEntitySet? EntitySet { get; }

        internal IEdmSingleton? Singleton { get; }

        internal IEdmOperation? Operation { get; }

        internal ODataRouteActionType ActionType { get; }

        internal ODataUrlKeyDelimiter UrlKeyDelimiter { get; }

        internal bool IsRouteExcluded { get; }

        internal bool IsAttributeRouted => routeAttribute != null;

        internal bool IsOperation => Operation != null;

        internal bool IsBound => IsOperation && ( EntitySet != null || Singleton != null );

        internal bool AllowUnqualifiedEnum => Services.GetRequiredService<ODataUriResolver>() is StringAsEnumResolver;

        internal IODataPathTemplateHandler PathTemplateHandler
        {
            get
            {
                if ( templateHandler == null )
                {
                    var conventions = Services.GetRequiredService<IEnumerable<IODataRoutingConvention>>();
                    var attribute = conventions.OfType<AttributeRoutingConvention>().FirstOrDefault();
                    templateHandler = attribute?.ODataPathTemplateHandler ?? new DefaultODataPathHandler();
                }

                return templateHandler;
            }
        }

        IEnumerable<string> GetHttpMethods( HttpActionDescriptor action ) => action.GetHttpMethods( route ).Select( m => m.Method );

        void ConvertODataActionParametersToTypedModel( IModelTypeBuilder modelTypeBuilder, IEdmAction action, string controllerName )
        {
            for ( var i = 0; i < ParameterDescriptions.Count; i++ )
            {
                var description = ParameterDescriptions[i];
                var parameter = description.ParameterDescriptor;

                if ( parameter != null && parameter.ParameterType.IsODataActionParameters() )
                {
                    var parameterType = modelTypeBuilder.NewActionParameters( Services, action, ApiVersion, controllerName );
                    description.ParameterDescriptor = new ODataModelBoundParameterDescriptor( parameter, parameterType );
                    break;
                }
            }
        }

        internal ODataRouteActionType GetActionType( HttpActionDescriptor action )
        {
            if ( EntitySet == null && Singleton == null )
            {
                if ( Operation == null )
                {
                    return ODataRouteActionType.Unknown;
                }
                else if ( !Operation.IsBound )
                {
                    return ODataRouteActionType.UnboundOperation;
                }
            }
            else if ( Operation == null )
            {
                if ( IsActionOrFunction( EntitySet, Singleton, action.ActionName, GetHttpMethods( action ) ) )
                {
                    return ODataRouteActionType.Unknown;
                }
                else if ( Singleton == null )
                {
                    return ODataRouteActionType.EntitySet;
                }
                else
                {
                    return ODataRouteActionType.Singleton;
                }
            }

            if ( Operation.IsBound )
            {
                return ODataRouteActionType.BoundOperation;
            }

            return ODataRouteActionType.UnboundOperation;
        }

        // Slash became the default 4/18/2018
        // REF: https://github.com/OData/WebApi/pull/1393
        static ODataUrlKeyDelimiter UrlKeyDelimiterOrDefault( ODataUrlKeyDelimiter? urlKeyDelimiter ) => urlKeyDelimiter ?? Slash;

        // REF: https://github.com/OData/WebApi/blob/master/src/Microsoft.AspNet.OData.Shared/Routing/Conventions/ActionRoutingConvention.cs
        // REF: https://github.com/OData/WebApi/blob/master/src/Microsoft.AspNet.OData.Shared/Routing/Conventions/FunctionRoutingConvention.cs
        // REF: https://github.com/OData/WebApi/blob/master/src/Microsoft.AspNet.OData.Shared/Routing/Conventions/EntitySetRoutingConvention.cs
        // REF: https://github.com/OData/WebApi/blob/master/src/Microsoft.AspNet.OData.Shared/Routing/Conventions/EntityRoutingConvention.cs
        // REF: https://github.com/OData/WebApi/blob/master/src/Microsoft.AspNet.OData.Shared/Routing/Conventions/SingletonRoutingConvention.cs
        static bool IsActionOrFunction( IEdmEntitySet? entitySet, IEdmSingleton? singleton, string actionName, IEnumerable<string> methods )
        {
            using var iterator = methods.GetEnumerator();

            if ( !iterator.MoveNext() )
            {
                return false;
            }

            var method = iterator.Current;

            if ( iterator.MoveNext() )
            {
                return false;
            }

            if ( entitySet == null && singleton == null )
            {
                return true;
            }

            const string ActionMethod = "Post";
            const string FunctionMethod = "Get";

            if ( ActionMethod.Equals( method, OrdinalIgnoreCase ) && actionName != ActionMethod )
            {
                if ( actionName.StartsWith( "CreateRef", Ordinal ) ||
                   ( entitySet != null && actionName == ( ActionMethod + entitySet.Name ) ) )
                {
                    return false;
                }

                return !IsNavigationPropertyLink( entitySet, singleton, actionName, ActionMethod );
            }
            else if ( FunctionMethod.Equals( method, OrdinalIgnoreCase ) && actionName != FunctionMethod )
            {
                if ( actionName.StartsWith( "GetRef", Ordinal ) ||
                   ( entitySet != null && actionName == ( ActionMethod + entitySet.Name ) ) )
                {
                    return false;
                }

                return !IsNavigationPropertyLink( entitySet, singleton, actionName, FunctionMethod );
            }

            return false;
        }

        static bool IsNavigationPropertyLink( IEdmEntitySet? entitySet, IEdmSingleton? singleton, string actionName, string method )
        {
            var entities = new List<IEdmEntityType>( capacity: 2 );

            if ( entitySet != null )
            {
                entities.Add( entitySet.EntityType() );
            }

            if ( singleton != null )
            {
                var entity = singleton.EntityType();

                if ( entities.Count == 0 || !entities[0].Equals( entity ) )
                {
                    entities.Add( entity );
                }
            }

            for ( var i = 0; i < entities.Count; i++ )
            {
                var entity = entities[i];

                if ( actionName == ( method + entity.Name ) )
                {
                    return true;
                }

                foreach ( var property in entity.NavigationProperties() )
                {
                    if ( actionName.StartsWith( method + property.Name, OrdinalIgnoreCase ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        IEdmOperation? ResolveOperation( IEdmEntityContainer container, string name )
        {
            var import = container.FindOperationImports( name ).SingleOrDefault();

            if ( import != null )
            {
                return import.Operation;
            }

            var qualifiedName = container.Namespace + "." + name;
            var entities = new List<IEdmType>( capacity: 2 );

            if ( Singleton != null )
            {
                entities.Add( Singleton.EntityType() );
            }

            if ( EntitySet != null )
            {
                var entity = EntitySet.EntityType();

                if ( entities.Count == 0 || !entities[0].Equals( entity ) )
                {
                    entities.Add( entity );
                }
            }

            for ( var i = 0; i < entities.Count; i++ )
            {
                var operation = EdmModel.FindBoundOperations( qualifiedName, entities[i] ).SingleOrDefault();

                if ( operation != null )
                {
                    return operation;
                }
            }

            return EdmModel.FindDeclaredOperations( qualifiedName ).SingleOrDefault();
        }

        sealed class FixedEdmModelServiceProviderDecorator : IServiceProvider
        {
            readonly IServiceProvider decorated;
            readonly IEdmModel edmModel;

            internal FixedEdmModelServiceProviderDecorator( IServiceProvider decorated, IEdmModel edmModel )
            {
                this.decorated = decorated;
                this.edmModel = edmModel;
            }

            public object GetService( Type serviceType ) =>
                serviceType == typeof( IEdmModel ) ? edmModel : decorated.GetService( serviceType );
        }
    }
}