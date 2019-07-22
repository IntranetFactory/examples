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
            issue.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "id" });
            issue.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "title" });

            AddStatus(issue);
            schema.RegisterTypes(issue);

            var task = new ObjectGraphType { Name = "Task" };
            task.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "id" });
            task.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "title" });
            task.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "description" });
            task.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "date" });

            schema.RegisterTypes(task);

            var taskStatus = new ObjectGraphType { Name = "TaskStatus" };
            taskStatus.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "title" });
            taskStatus.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "link" });
            taskStatus.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "linkLabel" });
            taskStatus.AddField(new FieldType { ResolvedType = new BooleanGraphType(), Name = "actionable" });
            taskStatus.AddField(new FieldType { ResolvedType = new IntGraphType(), Name = "value" });
            taskStatus.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "color" });
            taskStatus.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "date" });
            taskStatus.AddField(new FieldType { ResolvedType = new StringGraphType(), Name = "description" });

            AddItems(taskStatus, task);
            schema.RegisterTypes(taskStatus);

            // add 'taskState' query to the schema
            var root = new ObjectGraphType ();
            root.Name = "Root";
            root.AddField(new FieldType { ResolvedType = taskStatus, Name = "taskState"});
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
                if (f.Name.Contains("State"))
                {
                    FieldType field = new FieldType();
                    field.Name = f.Name;
                    field.ResolvedType = new NonNullGraphType(f.ResolvedType);
                    field.Resolver = new FuncFieldResolver<dynamic>(ctx =>
                    {
                        return ReturnStatus(f.Name);
                    });
                    if (f.Arguments != null)
                    {
                        field.Arguments = new QueryArguments(
                         f.Arguments
                      );
                    }
                    return field;
                }
                else
                {
                    FieldType field = new FieldType();
                    field.Name = f.Name;
                    field.ResolvedType = new NonNullGraphType(f.ResolvedType);
                    field.Resolver = new FuncFieldResolver<List<dynamic>>(ctx =>
                    {
                        return ReturnList(f.Name);
                    });
                    if (f.Arguments != null)
                    {
                        field.Arguments = new QueryArguments(
                         f.Arguments
                      );
                    }
                    return field;
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
                else if (name.Contains("task"))
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
            void AddStatus(ObjectGraphType typeToEdit)
            {
                // title already exists in Issue we should probably check for each field if it exists in type
                //typeToEdit.Field(new StringGraphType().GetType(), "title");
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
        }
    }
}
