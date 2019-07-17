using System;
using GraphQL.Types;
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
              type Task {
                id: String
                title: String
                description: String
              }

              type Issue {
                id: String
                title: String
              }

              type Query {
                tasks: Task
              }

              type Query {
                issues: Issue
              }
            ");

            // loop through all types in schema to filter out custo types define by user
            var allTypes = schema.AllTypes.ToList();
            for (int i = 0; i < allTypes.Count; i++)
            {
                Type t = allTypes[i].GetType();

                // types defined by user are ObjectGraphType and we ignore query as we process it later and differently
                if (t == typeof(ObjectGraphType) && allTypes[i].Name != "Query")
                {
                    var myType = schema.FindType(allTypes[i].Name) as ObjectGraphType;

                    // register fields for Task
                    foreach (FieldType f in myType.Fields)
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

            // register queries and parameters
            foreach (FieldType f in schema.Query.Fields)
            {
                AddField(
                new FieldType()
                {
                    Name = f.Name + "xx",
                    ResolvedType = new ListGraphType(f.ResolvedType),
                    Arguments = new QueryArguments(
                        f.Arguments
                    ),

                    // right now all queries return same result
                    // this should be discussed
                    Resolver = new FuncFieldResolver<List<dynamic>>(ctx =>
                    {
                        return ReturnData(f.Name);
                    })
                });
            }

            List<dynamic> ReturnData(string name)
            {
                List<dynamic> dynamicList = new List<dynamic>();

                List<dynamic> listToReturn = null;

                if (name == "issues")
                {
                    listToReturn = data.issueTestList;
                }
                else if (name == "tasks")
                {
                    listToReturn = data.taskTestList;
                }

                foreach (var item in listToReturn)
                {
                    dynamic jo = new SimpleJson.JsonObject();
                    jo.Id = item.Id;
                    jo.id = item.Id;
                    jo.title = item.Title;
                    dynamicList.Add(jo);
                }

                return dynamicList;
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
