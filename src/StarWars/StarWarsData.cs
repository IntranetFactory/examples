using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using StarWars.Types;

namespace StarWars
{
    public class StarWarsData
    {
        private readonly List<Issue> _issues = new List<Issue>();
        private readonly List<Droid> _droids = new List<Droid>();

        public StarWarsData()
        {
            _issues.Add(new Issue
            {
                Id = "1",
                Name = "Issue1",
                Description = "desc1",
                Date = DateTime.Now.ToString(),
                Link = "link1"
            });
            _issues.Add(new Issue
            {
                Id = "2",
                Name = "Issue2",
                Description = "desc2",
                Date = DateTime.Now.ToString(),
                Link = "link2"
            });
            _issues.Add(new Issue
            {
                Id = "3",
                Name = "Issue3",
                Description = "desc3",
                Date = DateTime.Now.ToString(),
                Link = "link3"
            });
            _issues.Add(new Issue
            {
                Id = "4",
                Name = "Issue4",
                Description = "desc4",
                Date = DateTime.Now.ToString(),
                Link = "link4"
            });
            _issues.Add(new Issue
            {
                Id = "5",
                Name = "Issue5",
                Description = "desc5",
                Date = DateTime.Now.ToString(),
                Link = "link5"
            });

            _droids.Add(new Droid
            {
                Id = "3",
                Name = "R2-D2a",
                Friends = new[] { "1", "2" },
                AppearsIn = new[] { 4, 5, 6 },
                PrimaryFunction = "Astromech"
            });
            _droids.Add(new Droid
            {
                Id = "4",
                Name = "C-3PO",
                AppearsIn = new[] { 4, 5, 6 },
                PrimaryFunction = "Protocol"
            });
        }

        public IEnumerable<Entity> GetFriends(Entity character)
        {
            if (character == null)
            {
                return null;
            }

            var friends = new List<Entity>();
            var lookup = character.Friends;
            if (lookup != null)
            {
                _issues.Where(h => lookup.Contains(h.Id)).Apply(friends.Add);
                _droids.Where(d => lookup.Contains(d.Id)).Apply(friends.Add);
            }
            return friends;
        }

        public Task<Issue> GetIssueByIdAsync(string id)
        {
            return Task.FromResult(_issues.FirstOrDefault(h => h.Id == id));
        }

        public Task<List<Issue>> GetAllIssues()
        {
            return Task.FromResult(_issues.FindAll(i => i.Id != null));
        }

        public Task<Droid> GetDroidByIdAsync(string id)
        {
            return Task.FromResult(_droids.FirstOrDefault(h => h.Id == id));
        }

        public Issue AddHuman(Issue human)
        {
            human.Id = Guid.NewGuid().ToString();
            _issues.Add(human);
            return human;
        }
    }
}
