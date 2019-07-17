using GraphQL.Types;

namespace Assistant
{
    /// <example>
    /// This is an example JSON request for a mutation
    /// {
    ///   "query": "mutation ($human:HumanInput!){ createHuman(human: $human) { id name } }",
    ///   "variables": {
    ///     "human": {
    ///       "name": "Boba Fett"
    ///     }
    ///   }
    /// }
    /// </example>
    public class AssistantMutation : ObjectGraphType
    {
        public AssistantMutation(AssistantData data)
        {
            Name = "Mutation";
            /*
            Field<IssueType>(
                "createHuman",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IssueInputType>> {Name = "human"}
                ),
                resolve: context =>
                {
                    var human = context.GetArgument<issue>("human");
                    return data.AddIssue(human);
                });
                */
        }
    }
}
