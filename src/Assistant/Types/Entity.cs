namespace Assistant.Types
{
    public abstract class Entity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
    }

    public class issue : Entity
    {
    }
}
