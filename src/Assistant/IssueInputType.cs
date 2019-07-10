using GraphQL.Types;
using Assistant.Types;

namespace Assistant
{
    public class IssueInputType : InputObjectGraphType<issue>
    {
        public IssueInputType()
        {
            Name = "IssueInput";
            Field(x => x.Title);
        }
    }
}
