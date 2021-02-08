namespace Microsoft.AspNetCore.Mvc.Versioning
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Edm;
    using System;

    sealed class EdmModelDecorator : IServiceProvider
    {
        static readonly Type ModelType = typeof( IEdmModel );
        readonly IServiceProvider decorated;

        internal EdmModelDecorator( IServiceProvider decorated ) => this.decorated = decorated;

        public object? GetService( Type serviceType )
        {
            if ( serviceType != ModelType )
            {
                return decorated.GetService( serviceType );
            }

            var selector = decorated.GetRequiredService<IEdmModelSelector>();

            return selector.SelectModel( decorated );
        }
    }
}