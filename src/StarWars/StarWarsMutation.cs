using GraphQL.Types;
using StarWars.Types;

namespace StarWars
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
    public class StarWarsMutation : ObjectGraphType
    {
        public StarWarsMutation(StarWarsData data)
        {
            Name = "Mutation";

            Field<IssueType>(
                "createHuman",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IssueInputType>> {Name = "human"}
                ),
                resolve: context =>
                {
                    var human = context.GetArgument<Issue>("human");
                    return data.AddHuman(human);
                });
        }
    }
}
