using GraphQL.Types;
using Assistant.Types;

namespace Assistant
{
    /// <example>
    /*
    mutation{
      createIssue(issue:{title:"new title", description:"description", date:"2019-07-15T13:31:15Z", link:"google.com"}){
          title
      }
    }
    */
    /// </example>
    public class AssistantMutation : ObjectGraphType
    {
        public AssistantMutation(AssistantData data)
        {
            Name = "Mutation";

            Field<IssueType>(
                "createIssue",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IssueInputType>> { Name = "issue" }
                ),
                resolve: context =>
                {
                    var issue = context.GetArgument<issue>("issue");
                    return data.AddIssue(issue);
                });
        }
    }
}
