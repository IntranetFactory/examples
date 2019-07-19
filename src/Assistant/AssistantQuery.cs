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
                // create copy of the type to use it in the list
                // so we do not have Issue.List<Issue>
                ObjectGraphType tmp = new ObjectGraphType();
                foreach (FieldType f in typeToEdit.Fields)
                {
                    tmp.AddField(f);
                }

                // title already exists in Issue
                //typeToEdit.Field(new StringGraphType().GetType(), "title");
                typeToEdit.Field(new StringGraphType().GetType(), "link");
                typeToEdit.Field(new StringGraphType().GetType(), "linkLabel");
                typeToEdit.Field(new BooleanGraphType().GetType(), "actionable");
                typeToEdit.Field(new IntGraphType().GetType(), "value");
                typeToEdit.Field(new StringGraphType().GetType(), "color");
                typeToEdit.Field(new StringGraphType().GetType(), "date");
                typeToEdit.Field(new StringGraphType().GetType(), "description");
                //typeToEdit.Field(new ListGraphType(tmp).GetType(), "items");
                // this last line breaks schema, i'm trying to figure it out
            }
        }
    }
}
