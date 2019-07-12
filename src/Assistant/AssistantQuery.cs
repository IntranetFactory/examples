using System;
using GraphQL.Types;
using Assistant.Types;
using GraphQL.Resolvers;
using System.Collections.Generic;

namespace Assistant
{
    public class AssistantQuery : ObjectGraphType<object>
    {
        public AssistantQuery(AssistantData data)
        {
            Name = "Query";

            Field<ListGraphType<IssueType>>(
                "myissuesstatic",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "startdate", Description = "Start date to filter by" },
                    new QueryArgument<StringGraphType> { Name = "enddate", Description = "End date to filter by" },
                    new QueryArgument<IntGraphType> { Name = "page", Description = "Page of the response" },
                    new QueryArgument<IntGraphType> { Name = "pagesize", Description = "Page size of the response" }
                ),

                resolve: context => data.GetIssuesFromStaticList()
            );

            //name Query does not exist


            new FieldType()
            {
                Name = "querydynamic",
                //ResolvedType = new ListGraphType(simulationType),
                Resolver = new FuncFieldResolver<List<dynamic>>(ctx =>
                {

                    // _db is database?
                    //var simulations = _db.GetCollection<dynamic>(simulationType.Name);

                    List<dynamic> result = new List<dynamic>();
                    SimpleJson.JsonObject d1 = new SimpleJson.JsonObject();
                    d1.Add("id", "id1");
                    d1.Add("title", "title2");

                    SimpleJson.JsonObject d2 = new SimpleJson.JsonObject();
                    d2.Add("id","id2");
                    d2.Add("title", "title2");

                    result.Add(d1);
                    result.Add(d2);

                    //var result = simulations.Find(GraphQL.Builders<dynamic>.Filter.Empty).Limit(10).ToList();
                    return result;
                })
            };
        }
    }
}
