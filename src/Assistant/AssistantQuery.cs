using System;
using GraphQL.Types;
using Assistant.Types;

namespace Assistant
{
    public class AssistantQuery : ObjectGraphType<object>
    {
        public AssistantQuery(AssistantData data)
        {
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
        }
    }
}
