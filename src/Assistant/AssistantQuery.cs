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
            issue.Field(new StringGraphType().GetType(), "id");
            issue.Field(new StringGraphType().GetType(), "title");

            AddStatus(issue);
            schema.RegisterTypes(issue);

            var task = new ObjectGraphType { Name = "Task" };
            task.Field(new StringGraphType().GetType(), "id");
            task.Field(new StringGraphType().GetType(), "title");
            task.Field(new StringGraphType().GetType(), "description");
            task.Field(new StringGraphType().GetType(), "date");

            schema.RegisterTypes(task);

            var taskStatus = new ObjectGraphType { Name = "TaskStatus" };
            taskStatus.Field(new StringGraphType().GetType(), "title");
            taskStatus.Field(new StringGraphType().GetType(), "link");
            taskStatus.Field(new StringGraphType().GetType(), "linkLabel");
            taskStatus.Field(new BooleanGraphType().GetType(), "actionable");
            taskStatus.Field(new IntGraphType().GetType(), "value");
            taskStatus.Field(new StringGraphType().GetType(), "color");
            taskStatus.Field(new StringGraphType().GetType(), "date");
            taskStatus.Field(new StringGraphType().GetType(), "description");

            AddItems(taskStatus, task);
            schema.RegisterTypes(taskStatus);

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
            void AddStatus(ObjectGraphType typeToEdit)
            {
                // title already exists in Issue we should probably check for each field if it exists in type
                //typeToEdit.Field(new StringGraphType().GetType(), "title");
                typeToEdit.Field(new StringGraphType().GetType(), "link");
                typeToEdit.Field(new StringGraphType().GetType(), "linkLabel");
                typeToEdit.Field(new BooleanGraphType().GetType(), "actionable");
                typeToEdit.Field(new IntGraphType().GetType(), "value");
                typeToEdit.Field(new StringGraphType().GetType(), "color");
                typeToEdit.Field(new StringGraphType().GetType(), "date");
                typeToEdit.Field(new StringGraphType().GetType(), "description");
            }

            void AddItems(ObjectGraphType typeToEdit, IGraphType typeOfItems)
            {
                // this is example from :
                //https://github.com/graphql-dotnet/graphql-dotnet/blob/e27623aaea70a73e1a5fa6dc49b3691d34059b05/src/GraphQL.Tests/Execution/RegisteredInstanceTests.cs#L51
                // it's not working
                /*
                var person = new ObjectGraphType { Name = "Person" };
                person.Field(new StringGraphType().GetType(), "name");
                person.Field(
                    new ListGraphType(person).GetType(),
                    "friends",
                    "description of the person",
                    arguments: new QueryArguments(
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name", Description = "Id of the issue" }
                    ),
                    resolve: ctx => new[] { new { Name = "Jaime" }, new { Name = "Joe" } });

                schema.RegisterTypes(person);

                var root = new ObjectGraphType { Name = "Root" };
                root.Field(person.GetType(), "hero", resolve: ctx => ctx.RootValue);
                schema.Query = root;
                */

                // it should be this sipmle to add new ListGraphType from everything that I saw
                // but its not working
                typeToEdit.Field(new ListGraphType(typeOfItems).GetType(), "items");

                schema.RegisterTypes(typeToEdit);
            }
        }
    }
}
