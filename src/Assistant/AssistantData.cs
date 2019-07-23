using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assistant
{

    public class AssistantData
    {
        public List<dynamic> issueTestList = new List<dynamic>();
        public List<dynamic> taskTestList = new List<dynamic>();
        public Schema schema;

        public AssistantData()
        {
            schema = new Schema();

            dynamic issue1 = new SimpleJson.JsonObject();
            issue1.Id = "1";
            issue1.Title = "Issue1";
            issue1.Description = "desc1";
            issue1.Date = DateTime.Now.ToJsonString();
            issue1.Link = "link1";

            dynamic issue2 = new SimpleJson.JsonObject();
            issue2.Id = "2";
            issue2.Title = "Issue2";
            issue2.Description = "desc2";
            issue2.Date = DateTime.Now.ToJsonString();
            issue2.Link = "link2";

            dynamic issue3 = new SimpleJson.JsonObject();
            issue3.Id = "3";
            issue3.Title = "Issue3";
            issue3.Description = "desc3";
            issue3.Date = DateTime.Now.ToJsonString();
            issue3.Link = "link3";

            dynamic issue4 = new SimpleJson.JsonObject();
            issue4.Id = "4";
            issue4.Title = "Issue4";
            issue4.Description = "desc4";
            issue4.Date = DateTime.Now.ToJsonString();
            issue4.Link = "link4";

            dynamic issue5 = new SimpleJson.JsonObject();
            issue5.Id = "5";
            issue5.Title = "Issue5";
            issue5.Description = "desc5";
            issue5.Date = DateTime.Now.ToJsonString();
            issue5.Link = "link5";

            issueTestList.Add(issue1);
            issueTestList.Add(issue2);
            issueTestList.Add(issue3);
            issueTestList.Add(issue4);
            issueTestList.Add(issue5);

            dynamic task1 = new SimpleJson.JsonObject();
            task1.Id = "1";
            task1.Title = "Task1";
            task1.Description = "desc1";
            task1.Date = DateTime.Now.ToJsonString();
            task1.Link = "link1";

            dynamic task2 = new SimpleJson.JsonObject();
            task2.Id = "2";
            task2.Title = "Task2";
            task2.Description = "desc2";
            task2.Date = DateTime.Now.ToJsonString();
            task2.Link = "link2";

            dynamic task3 = new SimpleJson.JsonObject();
            task3.Id = "3";
            task3.Title = "Task3";
            task3.Description = "desc3";
            task3.Date = DateTime.Now.ToJsonString();
            task3.Link = "link3";

            dynamic task4 = new SimpleJson.JsonObject();
            task4.Id = "4";
            task4.Title = "Task4";
            task4.Description = "desc4";
            task4.Date = DateTime.Now.ToJsonString();
            task4.Link = "link4";

            dynamic task5 = new SimpleJson.JsonObject();
            task5.Id = "5";
            task5.Title = "Task5";
            task5.Description = "desc5";
            task5.Date = DateTime.Now.ToJsonString();
            task5.Link = "link5";

            taskTestList.Add(task1);
            taskTestList.Add(task2);
            taskTestList.Add(task3);
            taskTestList.Add(task4);
            taskTestList.Add(task5);
        }

        public Task<dynamic> GetIssueByIdAsync(string id)
        {
            return Task.FromResult(issueTestList.FirstOrDefault(h => h.Id == id));
        }

        public Task<List<dynamic>> GetIssuesFromEndpoint(string startDate, string endDate, int page, int pageSize)
        {
            List<dynamic> issues = new List<dynamic>();

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

                    dynamic issue = new SimpleJson.JsonObject();
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

        public Task<List<dynamic>> GetOpenIssuesFromEndpoint(string startDate, string endDate, int page, int pageSize)
        {
            List<dynamic> issues = new List<dynamic>();

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

                    dynamic issue = new SimpleJson.JsonObject();
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

        public dynamic GetItemsFromStaticList(string name, ResolveFieldContext ctx)
        {
            string _startDate = "";
            string _endDate = "";
            int first = 0;
            int offset = 0;

            if (ctx.HasArgument("startdate") && ctx.HasArgument("enddate"))
            {
                _startDate = ctx.GetArgument<string>("startdate");
                _endDate = ctx.GetArgument<string>("enddate");
            }

            if (ctx.HasArgument("first"))
            {
                first = ctx.GetArgument<int>("first");
            }

            if (ctx.HasArgument("offset"))
            {
                offset = ctx.GetArgument<int>("offset");
            }

            List<dynamic> listToReturn = null;

            if (name.Contains("issue"))
            {
                listToReturn = issueTestList;
            }
            else if (name.Contains("task"))
            {
                listToReturn = taskTestList;
            }

            // filter by daterange
            List<dynamic> filteredByDateTime = null;
            if (!string.IsNullOrEmpty(_startDate) && !string.IsNullOrEmpty(_endDate))
            {
                DateTime startDate = _startDate.ToDateTime();
                DateTime endDate = _endDate.ToDateTime();

                filteredByDateTime = new List<dynamic>();
                for (int i = 0; i < listToReturn.Count; i++)
                {
                    string s = listToReturn[i].Date.ToString();
                    DateTime d = s.ToDateTime();
                    if (d > startDate && d < endDate)
                    {
                        filteredByDateTime.Add(listToReturn[i]);
                    }
                }
            }

            if (filteredByDateTime == null)
            {
                filteredByDateTime = listToReturn;
            }

            int page = 0;
            if (offset > 0)
            {
                page = offset / first;
            }
            int pageSize = (offset + first) - (page * first);
            int waste = pageSize - first;

            List<dynamic> paginatedItems = null;
            if (first > 0)
            {
                paginatedItems = new List<dynamic>();

                for (int i = offset; i < offset + first; i++)
                {
                    if (i >= filteredByDateTime.Count)
                    {
                        break;
                    }
                    paginatedItems.Add(filteredByDateTime[i]);
                }
            }
            dynamic response = new SimpleJson.JsonObject();
            response.value = filteredByDateTime.Count;

            if (paginatedItems == null)
            {
                paginatedItems = filteredByDateTime;
            }

            List<dynamic> items = new List<dynamic>();
            foreach (var item in paginatedItems)
            {
                dynamic jo = new SimpleJson.JsonObject();
                jo.id = item.Id;
                jo.title = item.Title;
                jo.description = item.Description;
                jo.date = item.Date;
                items.Add(jo);
            }
            response.items = items;

            return response;
        }

        public dynamic AddTask(ResolveFieldContext ctx)
        {
            string title = "";
            string description = "";
            string date = "";

            if (ctx.HasArgument("title"))
            {
                title = ctx.GetArgument<string>("title");
            }

            if (ctx.HasArgument("description"))
            {
                description = ctx.GetArgument<string>("description");
            }

            if (ctx.HasArgument("date"))
            {
                date = ctx.GetArgument<string>("date");
            }

            dynamic task = new SimpleJson.JsonObject();
            task.id = Guid.NewGuid().ToString();
            task.title = title;
            task.description = description;
            task.date = date;

            return task;
        }
    }
}
