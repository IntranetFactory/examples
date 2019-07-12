namespace Assistant.Types
{
    public abstract class Entity
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class issue : Entity
    {
    }
}
