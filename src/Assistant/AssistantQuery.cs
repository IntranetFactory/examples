using System;
using GraphQL.Types;
using GraphQL.Resolvers;
using System.Collections.Generic;

namespace Assistant
{
    public class AssistantQuery : ObjectGraphType<object>
    {
        public AssistantQuery(AssistantData data)
        {
            var user = new ObjectGraphType { Name = "User" };
            AddUserProperties(user);
            data.schema.RegisterTypes(user);

            var issue = new ObjectGraphType { Name = "Issue" };
            AddStandardItemProperties(issue);
            data.schema.RegisterTypes(issue);

            var issueStatus = new ObjectGraphType { Name = "IssueStatus" };
            AddStatus(issueStatus);
            AddItems(issueStatus, issue);
            data.schema.RegisterTypes(issueStatus);

            var task = new ObjectGraphType { Name = "Task" };
            AddStandardItemProperties(task);
            AddCreatedBy(task, user);
            AddAssignedTo(task, user);
            data.schema.RegisterTypes(task);

            var taskStatus = new ObjectGraphType { Name = "TaskStatus" };
            AddStatus(taskStatus);
            AddItems(taskStatus, task);
            data.schema.RegisterTypes(taskStatus);

            var root = new ObjectGraphType();
            root.Name = "Root";

            FieldType taskState = new FieldType();
            taskState.Name = "taskState";
            taskState.ResolvedType = taskStatus;
            taskState.Arguments = new QueryArguments();
            AddDateRange(taskState);
            AddPagination(taskState);
            root.AddField(taskState);

            FieldType issueState = new FieldType();
            issueState.Name = "issueState";
            issueState.ResolvedType = issueStatus;
            issueState.Arguments = new QueryArguments();
            AddDateRange(issueState);
            AddPagination(issueState);
            root.AddField(issueState);

            data.schema.Query = root;

            if (data.schema.Query != null)
            {
                // register queries and parameters
                foreach (FieldType f in data.schema.Query.Fields)
                {
                    AddField(GetFieldType(f));
                }
            }

            // decides if it should return list or single dynamic object based on name of the query
            FieldType GetFieldType(FieldType f)
            {
                FieldType field = new FieldType();
                field.Name = f.Name;
                field.ResolvedType = new NonNullGraphType(f.ResolvedType);
                field.Arguments = new QueryArguments(f.Arguments);

                if (f.Name.Contains("State"))
                {
                    field.Resolver = new FuncFieldResolver<dynamic>(ctx =>
                    {
                        return ReturnStatus(f.Name, ctx);
                    });

                    return field;
                }
                else if (f.Name.Contains("createTask"))
                {
                    field.Resolver = new FuncFieldResolver<List<dynamic>>(context =>
                    {
                        //var issue = context.GetArgument(schema.FindType("Task"), "task");
                        return data.AddTask(context);
                    });

                    return field;
                }
                else
                {
                    field.Resolver = new FuncFieldResolver<List<dynamic>>(ctx =>
                    {
                        return ReturnItems(f.Name, ctx);
                    });

                    return field;
                }
            }

            dynamic ReturnStatus(string name, ResolveFieldContext ctx)
            {
                string startDate = "";
                string endDate = "";
                int first = 0;
                int offset = 0;

                if (ctx.HasArgument("startdate") && ctx.HasArgument("enddate"))
                {
                    startDate = ctx.GetArgument<string>("startdate");
                    endDate = ctx.GetArgument<string>("enddate");
                }

                if (ctx.HasArgument("first"))
                {
                    first = ctx.GetArgument<int>("first");
                }

                if (ctx.HasArgument("offset"))
                {
                    offset = ctx.GetArgument<int>("offset");
                }

                dynamic response = new SimpleJson.JsonObject();

                dynamic staticData = data.GetItemsFromStaticList(name, ctx);

                response.items = staticData.items;
                response.value = staticData.value;
                response.title = name;
                response.link = "google.com";
                response.linkLabel = "all " + name;
                response.actionable = staticData.value > 0 ? true : false;
                response.color = "blue";
                response.date = new DateTime().ToJsonString();
                response.description = "description of the " + name;

                return response;
            }

            List<dynamic> ReturnItems(string name, ResolveFieldContext ctx)
            {
                dynamic staticData = data.GetItemsFromStaticList(name, ctx);

                return staticData.items;
            }

            void AddTypeField(ObjectGraphType ot, IGraphType t, string name)
            {
                FieldType f = new FieldType { ResolvedType = t, Name = name };

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

                ot.AddField(f);
            }

            void AddUserProperties(ObjectGraphType typeToEdit)
            {
                AddTypeField(typeToEdit, new StringGraphType(), "id");
                AddTypeField(typeToEdit, new StringGraphType(), "title");
            }

            void AddStandardItemProperties(ObjectGraphType typeToEdit)
            {
                AddTypeField(typeToEdit, new StringGraphType(), "id");
                AddTypeField(typeToEdit, new StringGraphType(), "title");
                AddTypeField(typeToEdit, new StringGraphType(), "description");
                AddTypeField(typeToEdit, new StringGraphType(), "date");
            }

            void AddStatus(ObjectGraphType typeToEdit)
            {
                AddTypeField(typeToEdit, new StringGraphType(), "title");
                AddTypeField(typeToEdit, new StringGraphType(), "link");
                AddTypeField(typeToEdit, new StringGraphType(), "linkLabel");
                AddTypeField(typeToEdit, new BooleanGraphType(), "actionable");
                AddTypeField(typeToEdit, new IntGraphType(), "value");
                AddTypeField(typeToEdit, new StringGraphType(), "color");
                AddTypeField(typeToEdit, new StringGraphType(), "date");
                AddTypeField(typeToEdit, new StringGraphType(), "description");
            }

            void AddItems(ObjectGraphType typeToEdit, IGraphType typeOfItems)
            {
                AddTypeField(typeToEdit, new ListGraphType(typeOfItems), "items");
            }

            void AddCreatedBy(ObjectGraphType typeToEdit, IGraphType userType)
            {
                AddTypeField(typeToEdit, userType, "createdBy");
            }

            void AddAssignedTo(ObjectGraphType typeToEdit, IGraphType typeOfItems)
            {
                AddTypeField(typeToEdit, new ListGraphType(typeOfItems), "assignedTo");
            }

            void AddDateRange(FieldType query)
            {
                query.Arguments.Add(new QueryArgument<StringGraphType> { Name = "startdate", Description = "Start date to filter by" });
                query.Arguments.Add(new QueryArgument<StringGraphType> { Name = "enddate", Description = "End date to filter by" });
            }

            void AddPagination(FieldType query)
            {
                query.Arguments.Add(new QueryArgument<IntGraphType> { Name = "first", Description = "Number of items to select." });
                query.Arguments.Add(new QueryArgument<IntGraphType> { Name = "offset", Description = "Number of items to skip." });
            }
        }
    }
}
