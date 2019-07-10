using GraphQL.Types;
using StarWars.Types;

namespace StarWars
{
    public class IssueInputType : InputObjectGraphType<issue>
    {
        public IssueInputType()
        {
            Name = "IssueInput";
            Field(x => x.Title);
            Field(x => x.HomePlanet, nullable: true);
        }
    }
}
