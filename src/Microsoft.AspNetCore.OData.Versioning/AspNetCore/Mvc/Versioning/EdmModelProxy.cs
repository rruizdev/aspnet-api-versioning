namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using System;
    using System.Collections.Generic;
    using static ODataRouteConfigurationState;

    sealed class EdmModelProxy : IEdmModel
    {
        readonly IEdmModel decorated;
        readonly ODataRouteConfiguration routeConfiguration;
        readonly IServiceProvider serviceProvider;

        internal EdmModelProxy(
            IEdmModel decorated,
            ODataRouteConfiguration routeConfiguration,
            IServiceProvider serviceProvider )
        {
            this.decorated = decorated;
            this.routeConfiguration = routeConfiguration;
            this.serviceProvider = serviceProvider;
        }

        IEdmModel Inner
        {
            get
            {
                if ( routeConfiguration.State == Configured )
                {
                    return serviceProvider.GetRequiredService<IEdmModel>();
                }

                return decorated;
            }
        }

        public IEnumerable<IEdmSchemaElement> SchemaElements => Inner.SchemaElements;

        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations => Inner.VocabularyAnnotations;

        public IEnumerable<IEdmModel> ReferencedModels => Inner.ReferencedModels;

        public IEnumerable<string> DeclaredNamespaces => Inner.DeclaredNamespaces;

        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager => Inner.DirectValueAnnotationsManager;

        public IEdmEntityContainer EntityContainer => Inner.EntityContainer;

        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations( IEdmType bindingType ) => Inner.FindDeclaredBoundOperations( bindingType );

        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations( string qualifiedName, IEdmType bindingType ) =>
            Inner.FindDeclaredBoundOperations( qualifiedName, bindingType );

        public IEnumerable<IEdmOperation> FindDeclaredOperations( string qualifiedName ) => Inner.FindDeclaredOperations( qualifiedName );

        public IEdmTerm FindDeclaredTerm( string qualifiedName ) => Inner.FindDeclaredTerm( qualifiedName );

        public IEdmSchemaType FindDeclaredType( string qualifiedName ) => Inner.FindDeclaredType( qualifiedName );

        public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations( IEdmVocabularyAnnotatable element ) =>
            Inner.FindDeclaredVocabularyAnnotations( element );

        public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes( IEdmStructuredType baseType ) =>
            Inner.FindDirectlyDerivedTypes( baseType );
    }
}