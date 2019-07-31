using System;
using System.Linq;
using GraphQL.Introspection;

namespace GraphQL.Conversion
{
    public class TransparentFieldNameConverter : IFieldNameConverter
    {
        public string NameFor(string field, Type parentType)
        {
            /*
            if (isIntrospectionType(parentType))
            {
                return field.ToCamelCase();
            }
            */
            return field.ToLowerInvariant() == "errors" || field.ToLowerInvariant() == "data"
            ? field.ToLowerInvariant()
            : field;
        }
        /*
        private bool isIntrospectionType(Type type)
        {
            return IntrospectionTypes.Contains(type);
        }
        */
    }
}
