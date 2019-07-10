using GraphQL.Types;

namespace Assistant.Types
{
    public class EntityInterface : InterfaceGraphType<Entity>
    {
        public EntityInterface()
        {
            Name = "Character";

            Field(d => d.Id).Description("The id of the character.");
            Field(d => d.Title, nullable: true).Description("The name of the character.");
            Field(d => d.Description, nullable: true).Description("The description of the character.");

            Field<ListGraphType<EntityInterface>>("friends");
            //Field<ListGraphType<EpisodeEnum>>("appearsIn", "Which movie they appear in.");
        }
    }
}
