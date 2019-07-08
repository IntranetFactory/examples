using System;
using GraphQL.Types;
using StarWars.Types;

namespace StarWars
{
    public class StarWarsQuery : ObjectGraphType<object>
    {
        public StarWarsQuery(StarWarsData data)
        {
            Name = "Query";

            Field<EntityInterface>("hero", resolve: context => data.GetDroidByIdAsync("3"));

            Field<IssueType>(
                "issue",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the issue" }
                ),
                resolve: context => data.GetIssueByIdAsync(context.GetArgument<string>("id"))
            );

            Field<ListGraphType<IssueType>>(
                "issues",
                resolve: context => data.GetAllIssues()
            );

            Func<ResolveFieldContext, string, object> func = (context, id) => data.GetDroidByIdAsync(id);

            FieldDelegate<DroidType>(
                "droid",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the droid" }
                ),
                resolve: func
            );
        }
    }
}
