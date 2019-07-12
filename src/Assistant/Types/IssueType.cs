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
            Field(h => h.Description, nullable: true).Description("The description of the issue.");
            Field(h => h.Date, nullable: true).Description("The date of the issue.");
            Field(h => h.Link, nullable: true).Description("The link of the issue.");


            // this file does not exist
            var schema = Schema.For(File.ReadAllText("GraphQL/schemas/schema.graphqls"));

            var simulationType = schema.FindType("Simulation") as ObjectGraphType;

            // foreach cant be used in this case it shows error
            simulationType.Fields.ForEach(f => f.Resolver = new FuncFieldResolver<object>(ctx =>
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
            }));

            RegisterTypes(schema.AllTypes.ToArray());

            //name Query does not exist
            Query.AddField(
                new FieldType()
                {
                    Name = "simulations",
                    ResolvedType = new ListGraphType(simulationType),
                    Resolver = new FuncFieldResolver<List<dynamic>>(ctx =>
                    {

                        // _db is database?
                        var simulations = _db.GetCollection<dynamic>(simulationType.Name);

                        // Builders is showing error
                        var result = simulations.Find(Builders<dynamic>.Filter.Empty).Limit(10).ToList();
                        return result;
                    })
                });

            Interface<EntityInterface>();
        }

        private void RegisterTypes(object p)
        {
            throw new NotImplementedException();
        }
    }
}
