using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using Assistant.Types;

namespace Assistant
{
    public class AssistantData
    {
        private readonly List<issue> testlist = new List<issue>();

        public AssistantData()
        {
            testlist.Add(new issue
            {
                Id = "1",
                Title = "Issue1",
                Description = "desc1",
                Date = DateTime.Now.ToString(),
                Link = "link1"
            });
            testlist.Add(new issue
            {
                Id = "2",
                Title = "Issue2",
                Description = "desc2",
                Date = DateTime.Now.ToString(),
                Link = "link2"
            });
            testlist.Add(new issue
            {
                Id = "3",
                Title = "Issue3",
                Description = "desc3",
                Date = DateTime.Now.ToString(),
                Link = "link3"
            });
            testlist.Add(new issue
            {
                Id = "4",
                Title = "Issue4",
                Description = "desc4",
                Date = DateTime.Now.ToString(),
                Link = "link4"
            });
            testlist.Add(new issue
            {
                Id = "5",
                Title = "Issue5",
                Description = "desc5",
                Date = DateTime.Now.ToString(),
                Link = "link5"
            });
        }

        public Task<issue> GetIssueByIdAsync(string id)
        {
            return Task.FromResult(testlist.FirstOrDefault(h => h.Id == id));
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

        public Task<List<issue>> GetOpenIssuesFromEndpoint(string startDate, string endDate, int page, int pageSize)
        {
            List<issue> issues = new List<issue>();

            string url = "http://localhost:2014/api/adenin.GateKeeper.Connector/briefing/issues-open?";

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
            DateTime? startDate = null;
            if (string.IsNullOrEmpty(_startDate))
            {
                startDate = _startDate.ToDateTime();
            }

            DateTime? endDate = null;
            if (string.IsNullOrEmpty(_endDate))
            {
                endDate = _endDate.ToDateTime();
            }

            // filter by daterange
            List<issue> filteredByDateTime = null;
            if (!string.IsNullOrEmpty(_startDate) && !string.IsNullOrEmpty(_endDate))
            {
                filteredByDateTime = new List<issue>();
                for (int i = 0; i < testlist.Count; i++)
                {
                    DateTime d = testlist[i].Date.ToDateTime();
                    if (d > startDate && d < endDate)
                    {
                        filteredByDateTime.Add(testlist[i]);
                    }
                }
            }

            if (filteredByDateTime == null)
            {
                filteredByDateTime = testlist;
            }

            List<issue> paginatedItems = null;
            if (page > 0 && pageSize > 0)
            {
                paginatedItems = new List<issue>();
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

            if (paginatedItems == null)
            {
                paginatedItems = filteredByDateTime;
            }

            return Task.FromResult(paginatedItems);
        }

        public issue AddIssue(issue _issue)
        {
            /*
            // send post request to gitlab connector to create Issue
            string url = "http://localhost:2014/api/adenin.GateKeeper.Connector/gitlab-connector/issue-handler";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("X-Gitlab-Token", "12345678");


            string stringData = ""; //place body here
            var data = Encoding.ASCII.GetBytes(stringData); // or UTF8

            webRequest.ContentLength = data.Length;

            var newStream = webRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            */
            // TODO : adde exception handling and show error messages to user
            testlist.Add(_issue);
            return _issue;
        }
    }
}
