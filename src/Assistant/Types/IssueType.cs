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

            foreach (FieldType f in Fields)
            {
                f.Resolver = new FuncFieldResolver<object>(ctx =>
                {
                    var o = ctx.Source as IDictionary<string, object>;
                    if (o == null)
                    {
                        return null;
                    }
                                       
                    if (!o.ContainsKey(ctx.FieldName))
                    {
                        return null;
                    }
                                       
                    return o[ctx.FieldName];

                });

            }



        }
    }
}
