using GraphQL.Types;
using StarWars.Types;

namespace StarWars
{
    public class IssueInputType : InputObjectGraphType<Issue>
    {
        public IssueInputType()
        {
            Name = "IssueInput";
            Field(x => x.Name);
            Field(x => x.HomePlanet, nullable: true);
        }
    }
}
