namespace Microsoft.AspNetCore.OData.Extensions
{
    using System;

    static class ServiceProviderExtensions
    {
        internal static IServiceProvider WithParent( this IServiceProvider serviceProvider, IServiceProvider parent ) =>
            new CompositeServiceProvider( serviceProvider, parent );

        internal static TService WithParent<TService>(
            this IServiceProvider serviceProvider,
            IServiceProvider parent,
            Func<IServiceProvider, TService> implementationFactory ) => implementationFactory( serviceProvider.WithParent( parent ) );

        sealed class CompositeServiceProvider : IServiceProvider
        {
            readonly IServiceProvider parent;
            readonly IServiceProvider child;

            internal CompositeServiceProvider( IServiceProvider child, IServiceProvider parent )
            {
                this.parent = parent;
                this.child = child;
            }

            public object? GetService( Type serviceType ) => child.GetService( serviceType ) ?? parent.GetService( serviceType );
        }
    }
}