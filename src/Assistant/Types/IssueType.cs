using GraphQL.Types;
using GraphQL.Resolvers;
using System.Collections.Generic;

namespace Assistant.Types
{
    public class IssueType : ObjectGraphType<item>
    {
        public IssueType(AssistantData data)
        {
            Name = "Issue";
            Description = "Issues you have.";

            Field(h => h.Id).Description("The id of the issue.");
            Field<StringGraphType>("Title");
        }
    }
}
