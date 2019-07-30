using GraphQL.Resolvers;
using GraphQL.Types;

namespace Assistant
{
    /// <example>
    /*
mutation{
  createTask(description:"desc",title:"title"){
    id,
    title,
    description
  }
}
    */
    /// </example>
    public class AssistantMutation : ObjectGraphType
    {
        public AssistantMutation(AssistantData data)
        {

            Name = "Mutation";

            FieldType createTask = new FieldType();
            createTask.Name = "createTask";
            createTask.ResolvedType = data.schema.FindType("Task");
            AddStandardCreateArguments(createTask);
            AddField(createTask);

            FieldType createIssue = new FieldType();
            createIssue.Name = "createIssue";
            createIssue.ResolvedType = data.schema.FindType("Issue");
            AddStandardCreateArguments(createIssue);
            AddField(createIssue);

            void AddStandardCreateArguments(FieldType t)
            {
                t.Arguments = new QueryArguments();
                t.Arguments.Add(new QueryArgument<StringGraphType> { Name = "title", Description = "its title" });
                t.Arguments.Add(new QueryArgument<StringGraphType> { Name = "description", Description = "its description" });
                t.Arguments.Add(new QueryArgument<StringGraphType> { Name = "date", Description = "its date" });

                t.Resolver = new FuncFieldResolver<dynamic>(context =>
                {
                    return data.ExecuteRequestPOST(t.Name, context);
                });
            }
        }
    }
}
