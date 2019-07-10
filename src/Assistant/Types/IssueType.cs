using GraphQL.Types;

namespace Assistant.Types
{
    public class IssueType : ObjectGraphType<issue>
    {
        public IssueType(AssistantData data)
        {
            Name = "Issue";
            Description = "Issues you have.";

            Field(h => h.Id).Description("The id of the issue.");
            Field(h => h.Title, nullable: true).Description("The name of the issue.");
            Field(h => h.Description, nullable: true).Description("The description of the issue.");
            Field(h => h.Date, nullable: true).Description("The date of the issue.");
            Field(h => h.Link, nullable: true).Description("The link of the issue.");

            Interface<EntityInterface>();
        }
    }
}
