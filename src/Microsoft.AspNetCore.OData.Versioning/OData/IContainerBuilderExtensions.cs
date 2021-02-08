namespace Microsoft.OData
{
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.OData.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OData.Edm;
    using Microsoft.OData.ModelBuilder;
    using System;
    using static Microsoft.OData.ServiceLifetime;

    /// <summary>
    /// Provides extension methods for the <see cref="IContainerBuilder"/> interface.
    /// </summary>
    public static class IContainerBuilderExtensions
    {
        /// <summary>
        /// Adds service API versioning to the specified container builder.
        /// </summary>
        /// <param name="builder">The extended <see cref="IContainerBuilder">container builder</see>.</param>
        /// <param name="prefix">The prefix associated with the container.</param>
        /// <param name="serviceProvider">The associated <see cref="IServiceProvider">service provider</see>.</param>
        /// <returns>The original <paramref name="builder"/>.</returns>
        [CLSCompliant( false )]
        public static IContainerBuilder AddApiVersioning(
            this IContainerBuilder builder,
            string? prefix,
            IServiceProvider serviceProvider ) =>
            builder
                .AddService( Transient, sp => sp.GetRequiredService<IEdmModelSelector>().SelectModel( sp.WithParent( serviceProvider ) ) )
                .AddService(
                    Singleton,
                    child => child.WithParent(
                        serviceProvider,
                        sp => (IEdmModelSelector) new EdmModelSelector(
                            sp.GetRequiredService<VersionedODataModelBuilder>().GetEdmModels( prefix ),
                            sp.GetRequiredService<IOptions<ApiVersioningOptions>>().Value.DefaultApiVersion ) ) );
    }
}