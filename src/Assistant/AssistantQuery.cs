using System;
using GraphQL.Types;
using Assistant.Types;
using GraphQL.Resolvers;
using System.Linq;
using System.Collections.Generic;

namespace Assistant
{
    public class AssistantQuery : ObjectGraphType<object>
    {
        public AssistantQuery(AssistantData data)
        {
            var schema = Schema.For(@"
              type Issue {
                id: String
                title: String
              }

              type Query {
                issue(id: String!): Issue
              }

              type Query {
                issues: Issue
              }
            ");

            var simulationType = schema.FindType("Issue") as ObjectGraphType;

            // register types
            foreach (FieldType f in simulationType.Fields)
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

            // register queries and parameters
            foreach (FieldType f in schema.Query.Fields)
            {
                AddField(
                new FieldType()
                {
                    Name = f.Name,
                    ResolvedType = new ListGraphType(f.ResolvedType),
                    Arguments = new QueryArguments(
                        f.Arguments
                    ),

                    // right now all queries return same result
                    // this should be discussed
                    Resolver = new FuncFieldResolver<List<dynamic>>(ctx =>
                    {
                        List<dynamic> dynamicList = new List<dynamic>();
                        foreach (var item in data.testlist)
                        {
                            dynamic jo = new SimpleJson.JsonObject();
                            jo.Id = item.Id;
                            jo.id = item.Id;
                            jo.title = item.Title;
                            dynamicList.Add(jo);
                        }
                        return dynamicList;
                    })
                });
            }

            /*
            Name = "Query";

            Field<IssueType>(
                "issue",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "Id of the issue" }
                ),
                resolve: context => data.GetIssueByIdAsync(context.GetArgument<string>("id"))
            );

            Field<ListGraphType<IssueType>>(
                "myissues",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "startdate", Description = "Start date to filter by" },
                    new QueryArgument<StringGraphType> { Name = "enddate", Description = "End date to filter by" },
                    new QueryArgument<IntGraphType> { Name = "page", Description = "Page of the response" },
                    new QueryArgument<IntGraphType> { Name = "pagesize", Description = "Page size of the response" }
                ),

                resolve: context => data.GetIssuesFromEndpoint(context.GetArgument<string>("startdate"),
                                                    context.GetArgument<string>("enddate"),
                                                    context.GetArgument<int>("page"),
                                                    context.GetArgument<int>("pagesize"))
            );

            Field<ListGraphType<IssueType>>(
                "openissues",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "startdate", Description = "Start date to filter by" },
                    new QueryArgument<StringGraphType> { Name = "enddate", Description = "End date to filter by" },
                    new QueryArgument<IntGraphType> { Name = "page", Description = "Page of the response" },
                    new QueryArgument<IntGraphType> { Name = "pagesize", Description = "Page size of the response" }
                ),

                resolve: context => data.GetOpenIssuesFromEndpoint(context.GetArgument<string>("startdate"),
                                                    context.GetArgument<string>("enddate"),
                                                    context.GetArgument<int>("page"),
                                                    context.GetArgument<int>("pagesize"))
            );

            Field<ListGraphType<IssueType>>(
                "myissuesstatic",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "startdate", Description = "Start date to filter by" },
                    new QueryArgument<StringGraphType> { Name = "enddate", Description = "End date to filter by" },
                    new QueryArgument<IntGraphType> { Name = "page", Description = "Page of the response" },
                    new QueryArgument<IntGraphType> { Name = "pagesize", Description = "Page size of the response" }
                ),

                resolve: context => data.GetIssuesFromStaticList(context.GetArgument<string>("startdate"),
                                                    context.GetArgument<string>("enddate"),
                                                    context.GetArgument<int>("page"),
                                                    context.GetArgument<int>("pagesize"))
            );
            */
        }
    }
}
