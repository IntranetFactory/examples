using GraphQL.Types;

namespace StarWars.Types
{
    public class IssueType : ObjectGraphType<Issue>
    {
        public IssueType(StarWarsData data)
        {
            Name = "Issue";
            Description = "Issues you have.";

            Field(h => h.Id).Description("The id of the issue.");
            Field(h => h.Name, nullable: true).Description("The name of the issue.");
            Field(h => h.Description, nullable: true).Description("The description of the issue.");
            Field(h => h.Date, nullable: true).Description("The date of the issue.");
            Field(h => h.Link, nullable: true).Description("The link of the issue.");

            Field<ListGraphType<EntityInterface>>(
                "friends",
                resolve: context => data.GetFriends(context.Source)
            );
            Field<ListGraphType<EpisodeEnum>>("appearsIn", "Which movie they appear in.");

            Field(h => h.HomePlanet, nullable: true).Description("The home planet of the human.");

            Interface<EntityInterface>();
        }
    }
}
