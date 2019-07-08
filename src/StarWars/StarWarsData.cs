using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        public Task<List<Issue>> GetAllIssues(string startDate, string endDate, int page, int pageSize)
        {
            List<Issue> issues = new List<Issue>();

            string url = "http://localhost:2014/api/adenin.GateKeeper.Connector/briefing/issues?";

            if (startDate != "")
            {
                if (!url.EndsWith("?")) url += "&";
                url += "startDate=" + startDate;
            }

            if (endDate != "")
            {
                if (!url.EndsWith("?")) url += "&";
                url += "endDate=" + endDate;
            }

            if (page != 0)
            {
                if (!url.EndsWith("?")) url += "&";
                url += "page=" + page;
            }

            if (pageSize != 0)
            {
                if (!url.EndsWith("?")) url += "&";
                url += "pageSize=" + pageSize;
            }

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url as string);
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("X-ClusterKey", "utt0iaiyhraisdzma80i2uanrr8tyki4");
            webRequest.Headers.Add("X-UserName", "admin");
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                // Get the stream associated with the response.
                Stream receiveStream = webResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                // convert stream to JsonObject
                dynamic response = SimpleJson.SimpleJson.DeserializeObject(readStream.ReadToEnd());


                // read items from response and convert them to Issues
                for (int i = 0; i < response.Data.items.Count; i++)
                {
                    dynamic item = response.Data.items[i];

                    Issue issue = new Issue();
                    issue.Id = item.id;
                    issue.Name = item.title;
                    issue.Description = item.description;
                    issue.Date = item.date;
                    issue.Link = item.link;

                    issues.Add(issue);
                }

                webResponse.Close();
                readStream.Close();
            }

            return Task.FromResult(issues);
        }

        public Task<Droid> GetDroidByIdAsync(string id)
        {
            Droid d = _droids.FirstOrDefault(h => h.Id == id);
            return Task.FromResult(d);
        }

        public Issue AddHuman(Issue human)
        {
            human.Id = Guid.NewGuid().ToString();
            _issues.Add(human);
            return human;
        }
    }
}
