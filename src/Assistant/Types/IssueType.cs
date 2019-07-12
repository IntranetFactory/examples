using GraphQL.Resolvers;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assistant.Types
{
    public class IssueType : ObjectGraphType<issue>
    {
        public IssueType(AssistantData data)
        {
            Name = "Issue";
            Description = "Issues you have.";

            Field(h => h.Id).Description("The id of the issue.");
            Field(h => h.Title, nullable: true).Description("The name of the issue.");

            // foreach cant be used in this case it shows error
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

            // this function does not exist
            //RegisterTypes(schema.AllTypes.ToArray());

            Interface<EntityInterface>();
        }
    }
}
