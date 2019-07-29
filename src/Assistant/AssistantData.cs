using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assistant
{

    public class AssistantData
    {
        public List<dynamic> userTestList = new List<dynamic>();

        public List<dynamic> issueTestList = new List<dynamic>();
        public List<dynamic> taskTestList = new List<dynamic>();
        public Schema schema;

        public AssistantData()
        {
            schema = new Schema();

            dynamic user1 = new SimpleJson.JsonObject();
            user1.id = "u1";
            user1.title = "User1";

            dynamic user2 = new SimpleJson.JsonObject();
            user2.id = "u2";
            user2.title = "User2";

            dynamic user3 = new SimpleJson.JsonObject();
            user3.id = "u3";
            user3.title = "User3";

            dynamic user4 = new SimpleJson.JsonObject();
            user4.id = "u4";
            user4.title = "User4";

            dynamic user5 = new SimpleJson.JsonObject();
            user5.id = "u5";
            user5.title = "User5";

            userTestList.Add(user1);
            userTestList.Add(user2);
            userTestList.Add(user3);
            userTestList.Add(user4);
            userTestList.Add(user5);

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
            task1.createdBy = GetCreatedBy();
            task1.assignedTo = GetAssignedTo();

            dynamic task2 = new SimpleJson.JsonObject();
            task2.Id = "2";
            task2.Title = "Task2";
            task2.Description = "desc2";
            task2.Date = DateTime.Now.ToJsonString();
            task2.Link = "link2";
            task2.createdBy = GetCreatedBy();
            task2.assignedTo = GetAssignedTo();

            dynamic task3 = new SimpleJson.JsonObject();
            task3.Id = "3";
            task3.Title = "Task3";
            task3.Description = "desc3";
            task3.Date = DateTime.Now.ToJsonString();
            task3.Link = "link3";
            task3.createdBy = GetCreatedBy();
            task3.assignedTo = GetAssignedTo();

            dynamic task4 = new SimpleJson.JsonObject();
            task4.Id = "4";
            task4.Title = "Task4";
            task4.Description = "desc4";
            task4.Date = DateTime.Now.ToJsonString();
            task4.Link = "link4";
            task4.createdBy = GetCreatedBy();
            task4.assignedTo = GetAssignedTo();

            dynamic task5 = new SimpleJson.JsonObject();
            task5.Id = "5";
            task5.Title = "Task5";
            task5.Description = "desc5";
            task5.Date = DateTime.Now.ToJsonString();
            task5.Link = "link5";
            task5.createdBy = GetCreatedBy();
            task5.assignedTo = GetAssignedTo();

            taskTestList.Add(task1);
            taskTestList.Add(task2);
            taskTestList.Add(task3);
            taskTestList.Add(task4);
            taskTestList.Add(task5);
        }

        dynamic GetCreatedBy()
        {
            return userTestList[new Random().Next(0, userTestList.Count)];
        }

        List<dynamic> GetAssignedTo()
        {
            List<dynamic> assignees = new List<dynamic>();
            int noOfAssignees = new Random().Next(0, 3);
            for (int i = 0; i < noOfAssignees; i++)
            {
                int index = new Random().Next(0, userTestList.Count);
                if (!assignees.Contains(userTestList[index]))
                {
                    assignees.Add(userTestList[index]);
                }
            }

            return assignees;
        }

        public Task<dynamic> GetIssueByIdAsync(string id)
        {
            return Task.FromResult(issueTestList.FirstOrDefault(h => h.Id == id));
        }

        public List<dynamic> GetIssuesFromEndpoint(string startDate, string endDate, int first, int offset, ResolveFieldContext ctx)
        {
            List<dynamic> issues = new List<dynamic>();

            string url = "http://localhost:2014/api/adenin.GateKeeper.Connector/briefing/issuesaa?";

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

            int page = 0;
            if (offset > 0)
            {
                page = offset / first;
            }

            int pageSize = (offset + first) - (page * first);
            int waste = pageSize - first;

            //int waste = offset - first;

            if (!url.EndsWith("?")) url += "&";
            url += "page=" + page;

            if (!url.EndsWith("?")) url += "&";
            url += "pageSize=" + pageSize;

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


                //test code
                if (response.ErrorCode != 0)
                {
                    ctx.Errors.Add(new GraphQL.ExecutionError("ErrorCode: " + response.ErrorCode + ". ErrorText: " + response.Data.ErrorText));
                }

                if (response.Data.items == null)
                {
                    response.Data.items = new List<SimpleJson.JsonObject>();
                }
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

            return issues;
        }

        public List<dynamic> GetTasksFromEndpoint(string startDate, string endDate, int first, int offset)
        {
            List<dynamic> tasks = new List<dynamic>();

            string url = "http://localhost:2014/api/adenin.GateKeeper.Connector/briefing/tasks?";

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

            int page = 0;
            if (offset > 0)
            {
                page = offset / first;
            }

            int pageSize = (offset + first) - (page * first);
            int waste = pageSize - first;

            //int waste = offset - first;

            if (!url.EndsWith("?")) url += "&";
            url += "page=" + page;

            if (!url.EndsWith("?")) url += "&";
            url += "pageSize=" + pageSize;

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

                if (response.Data.items == null)
                {
                    response.Data.items = new List<SimpleJson.JsonObject>();
                }
                // read items from response and convert them to Issues
                for (int i = 0; i < response.Data.items.Count; i++)
                {
                    dynamic item = response.Data.items[i];

                    dynamic task = new SimpleJson.JsonObject();
                    task.Id = item.id;
                    task.Title = item.title;
                    task.Description = item.description;
                    task.Date = item.date;
                    task.Link = item.link;
                    task.createdBy = item.createdBy;
                    task.assignedTo = item.assignedTo;

                    tasks.Add(task);
                }

                webResponse.Close();
                readStream.Close();
            }

            return tasks;
        }

        public Task<dynamic> GetItems(string name, ResolveFieldContext ctx)
        {
            string startDate = "";
            string endDate = "";
            int first = 0;
            int offset = 0;

            if (ctx.HasArgument("startdate") && ctx.HasArgument("enddate"))
            {
                startDate = ctx.GetArgument<string>("startdate");
                endDate = ctx.GetArgument<string>("enddate");
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
                listToReturn = GetIssuesFromEndpoint(startDate, endDate, first, offset, ctx);
            }
            else if (name.Contains("task"))
            {
                listToReturn = GetTasksFromEndpoint(startDate, endDate, first, offset);
            }
            dynamic response = new SimpleJson.JsonObject();
            response.items = listToReturn;
            response.value = listToReturn.Count;

            List<dynamic> items = new List<dynamic>();
            foreach (var item in listToReturn)
            {
                dynamic jo = new SimpleJson.JsonObject();
                jo.id = item.Id;
                jo.title = item.Title;
                jo.description = item.Description;
                jo.date = item.Date;

                jo.createdBy = item.createdBy;
                jo.assignedTo = item.assignedTo;
                items.Add(jo);
            }
            response.items = items;

            return Task.FromResult(response as object);
        }

        public Task<dynamic> AddTask(ResolveFieldContext ctx)
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

            // send post request to gitlab connector to create Issue
            try
            {
                string url = "http://localhost:2014/api/adenin.GateKeeper.Connector/poc-connector/task-create";
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add("X-ClusterKey", "srb6enxednjjeeutjkpq4donu55r7of1");
                webRequest.Headers.Add("X-UserName", "admin");

                string stringData = SimpleJson.SimpleJson.SerializeObject(task); //place body here
                var data = Encoding.ASCII.GetBytes(stringData); // or UTF8
                webRequest.ContentLength = data.Length;
                var newStream = webRequest.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Get the stream associated with the response.
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                        // convert stream to JsonObject
                        dynamic res = SimpleJson.SimpleJson.DeserializeObject(readStream.ReadToEnd());

                        response.Close();
                        readStream.Close();
                    }
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var z = reader.ReadToEnd();
                    Console.WriteLine(z);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Something more serious happened
                // like for example you don't have network access
                // we cannot talk about a server exception here as
                // the server probably was never reached
            }

            return Task.FromResult(task as object);
        }
    }
}
