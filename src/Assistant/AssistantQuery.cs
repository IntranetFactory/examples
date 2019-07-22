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
            var schema = new Schema();

            var issue = new ObjectGraphType { Name = "Issue" };
            AddStandardItemProperties(issue);
            schema.RegisterTypes(issue);

            var issueStatus = new ObjectGraphType { Name = "IssueStatus" };
            AddStatus(issueStatus);
            AddItems(issueStatus, issue);
            schema.RegisterTypes(issueStatus);

            var task = new ObjectGraphType { Name = "Task" };
            AddStandardItemProperties(task);
            schema.RegisterTypes(task);

            var taskStatus = new ObjectGraphType { Name = "TaskStatus" };
            AddStatus(taskStatus);
            AddItems(taskStatus, task);
            schema.RegisterTypes(taskStatus);

            // add 'taskState' query to the schema
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

            schema.Query = root;

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

            if (schema.Query != null)
            {
                // register queries and parameters
                foreach (FieldType f in schema.Query.Fields)
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

            void AddStandardItemProperties(ObjectGraphType typeToEdit)
            {
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "id" });
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "title" });
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "description" });
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "date" });
            }

            void AddStatus(ObjectGraphType typeToEdit)
            {
                typeToEdit.Field(new StringGraphType().GetType(), "title");
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "link" });
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "linkLabel" });
                typeToEdit.AddField(new FieldType { ResolvedType = new BooleanGraphType(), Name = "actionable" });
                typeToEdit.AddField(new FieldType { ResolvedType = new IntGraphType(), Name = "value" });
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "color" });
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "date" });
                typeToEdit.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "description" });
            }

            void AddItems(ObjectGraphType typeToEdit, IGraphType typeOfItems)
            {
                FieldType itemsField = new FieldType();
                itemsField.Name = "items";
                itemsField.ResolvedType = new ListGraphType(typeOfItems);
                typeToEdit.AddField(itemsField);
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
