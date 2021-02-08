#if WEBAPI
namespace Microsoft.AspNet.OData
#else
namespace Microsoft.AspNetCore.OData
#endif
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    internal class PropertyDependency
    {
        internal PropertyDependency(
            EdmTypeKey dependentOnTypeKey,
            bool isCollection,
            string propertyName,
            IEnumerable<CustomAttributeBuilder> customAttributes )
        {
            DependentOnTypeKey = dependentOnTypeKey;
            PropertyName = propertyName;
            CustomAttributes = customAttributes.ToArray();
            IsCollection = isCollection;
        }

        internal TypeBuilder? DependentType { get; set;  }

        internal EdmTypeKey DependentOnTypeKey { get; }

        internal string PropertyName { get; }

        internal bool IsCollection { get; }

        internal IReadOnlyList<CustomAttributeBuilder> CustomAttributes { get; }
    }
}