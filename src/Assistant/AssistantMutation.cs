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
            createTask.Arguments = new QueryArguments();
            createTask.Arguments.Add(new QueryArgument<StringGraphType> { Name = "title", Description = "its title" });
            createTask.Arguments.Add(new QueryArgument<StringGraphType> { Name = "description", Description = "its description" });
            createTask.Arguments.Add(new QueryArgument<StringGraphType> { Name = "date", Description = "its date" });

            createTask.Resolver = new FuncFieldResolver<dynamic>(context =>
            {
                return data.AddTask(context);
            });

            AddField(createTask);
        }
    }
}
