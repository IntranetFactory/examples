using GraphQL.Types;
using Assistant.Types;

namespace Assistant
{
    public class IssueInputType : InputObjectGraphType<issue>
    {
        public IssueInputType()
        {
            Name = "IssueInput";
            // just example input for creating issue
            Field(x => x.Title);
            Field(x => x.Description);
            Field(x => x.Date);
            Field(x => x.Link);
        }
    }
}
