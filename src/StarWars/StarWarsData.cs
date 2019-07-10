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
        private readonly List<issue> _issues = new List<issue>();
        private readonly List<Droid> _droids = new List<Droid>();

        public StarWarsData()
        {
            _issues.Add(new issue
            {
                Id = "1",
                Title = "Issue1",
                Description = "desc1",
                Date = DateTime.Now.ToString(),
                Link = "link1"
            });
            _issues.Add(new issue
            {
                Id = "2",
                Title = "Issue2",
                Description = "desc2",
                Date = DateTime.Now.ToString(),
                Link = "link2"
            });
            _issues.Add(new issue
            {
                Id = "3",
                Title = "Issue3",
                Description = "desc3",
                Date = DateTime.Now.ToString(),
                Link = "link3"
            });
            _issues.Add(new issue
            {
                Id = "4",
                Title = "Issue4",
                Description = "desc4",
                Date = DateTime.Now.ToString(),
                Link = "link4"
            });
            _issues.Add(new issue
            {
                Id = "5",
                Title = "Issue5",
                Description = "desc5",
                Date = DateTime.Now.ToString(),
                Link = "link5"
            });

            _droids.Add(new Droid
            {
                Id = "3",
                Title = "R2-D2a",
                Friends = new[] { "1", "2" },
                AppearsIn = new[] { 4, 5, 6 },
                PrimaryFunction = "Astromech"
            });
            _droids.Add(new Droid
            {
                Id = "4",
                Title = "C-3PO",
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

        public Task<issue> GetIssueByIdAsync(string id)
        {
            return Task.FromResult(_issues.FirstOrDefault(h => h.Id == id));
        }

        public Task<List<issue>> GetIssuesFromEndpoint(string startDate, string endDate, int page, int pageSize)
        {
            List<issue> issues = new List<issue>();

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
            webRequest.Headers.Add("X-ClusterKey", "srb6enxednjjeeutjkpq4donu55r7of1");
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

                    issue issue = new issue();
                    issue.Id = item.id;
                    issue.Title = item.title;
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

        public Task<List<issue>> GetIssuesFromStaticList(string _startDate, string _endDate, int page, int pageSize)
        {
            List<issue> issues = new List<issue>();

            string url = "http://localhost:2014/api/adenin.GateKeeper.Connector/briefing/issues?";

            DateTime? startDate = null;
            if (_startDate != "")
            {
                startDate = _startDate.ToDateTime();
            }

            DateTime? endDate = null;
            if (_endDate != "")
            {
                endDate = _endDate.ToDateTime();
            }

            // filter by daterange
            List<issue> filteredByDateTime = new List<issue>();
            if (startDate != null && endDate != null)
            {
                for (int i = 0; i < _issues.Count; i++)
                {
                    DateTime d = _issues[i].Date.ToDateTime();
                    if (d > startDate && d < endDate)
                    {
                        filteredByDateTime.Add(_issues[i]);
                    }
                }
            }

            List<issue> paginatedItems = new List<issue>();
            if (page > 0 && pageSize > 0)
            {
                int offset = (page - 1) * pageSize;

                for (int i = offset; i < offset + pageSize; i++)
                {
                    if (i >= filteredByDateTime.Count)
                    {
                        break;
                    }
                    paginatedItems.Add(filteredByDateTime[i]);
                }
            }

            return Task.FromResult(paginatedItems);
        }

        public Task<Droid> GetDroidByIdAsync(string id)
        {
            Droid d = _droids.FirstOrDefault(h => h.Id == id);
            return Task.FromResult(d);
        }

        public issue AddHuman(issue human)
        {
            human.Id = Guid.NewGuid().ToString();
            _issues.Add(human);
            return human;
        }
    }
}
