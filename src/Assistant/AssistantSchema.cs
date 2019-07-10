using GraphQL;
using GraphQL.Types;

namespace Assistant
{
    public class AssistantSchema : Schema
    {
        public AssistantSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<AssistantQuery>();
            Mutation = resolver.Resolve<AssistantMutation>();
        }
    }
}
