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
            /*
            var schema = Schema.For(@"
            type taskStatus {
                title: String
                link: String
                linkLabel: String
                actionable: Boolean
                value: Int
                color: String
                date: String
                description: String
                items: [Task]
            }

            type issuesStatus {
                title: String
                link: String
                linkLabel: String
                actionable: Boolean
                value: Int
                color: String
                date: String
                description: String
                items: [Issue]
            }

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

            type Query {
                tasksState : taskStatus
            }

            type Query {
                issueState: issuesStatus
            }
            ");
            */
            var schema = new Schema();

            var person = new ObjectGraphType { Name = "Person" };
            person.Field(new StringGraphType().GetType(), "name");
            person.Field(
                new ListGraphType(new NonNullGraphType(person)).GetType(),
                "friends",
                resolve: ctx => new[] { new { Name = "Jaime" }, new { Name = "Joe" } });

            var root = new ObjectGraphType { Name = "Root" };
            root.Field(person.GetType(), "hero", resolve: ctx => ctx.RootValue);

            schema.Query = root;
            schema.RegisterType(person);

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
                AddField(GetFieldType(f));
            }

            // decides if it should return list or single dynamic object based on name of the query
            FieldType GetFieldType(FieldType f)
            {
                if (f.Name.Contains("State"))
                {
                    return new FieldType()
                    {
                        Name = f.Name,
                        ResolvedType = new NonNullGraphType(f.ResolvedType),
                        Arguments = new QueryArguments(
                         f.Arguments
                     ),

                        // right now all queries return same result
                        // this should be discussed
                        Resolver = new FuncFieldResolver<dynamic>(ctx =>
                        {
                            return ReturnStatus(f.Name);
                        })
                    };
                }
                else
                {
                    return new FieldType()
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
                            return ReturnList(f.Name);
                        })
                    };
                }
            }
            dynamic ReturnStatus(string name)
            {
                dynamic response = new SimpleJson.JsonObject();

                List<dynamic> items = ReturnList(name);

                response.items = items;
                response.value = items.Count;
                response.title = name;
                response.link = "google.com";
                response.linkLabel = "all " + name;
                response.actionable = items.Count > 1 ? true : false;
                response.color = "blue";
                response.date = new DateTime().ToJsonString();
                response.description = "description of the " + name;

                return response;
            }

            List<dynamic> ReturnList(string name)
            {
                List<dynamic> dynamicList = new List<dynamic>();
                List<dynamic> listToReturn = null;

                if (name.Contains("issue"))
                {
                    listToReturn = data.issueTestList;
                }
                else if (name.Contains("tasks"))
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
