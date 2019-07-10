namespace StarWars.Types
{
    public abstract class Entity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
        public string[] Friends { get; set; }
        public int[] AppearsIn { get; set; }
    }

    public class issue : Entity
    {
        public string HomePlanet { get; set; }
    }

    public class Droid : Entity
    {
        public string PrimaryFunction { get; set; }
    }
}
