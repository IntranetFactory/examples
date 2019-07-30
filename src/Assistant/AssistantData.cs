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

        public async Task<dynamic> ExecuteRequestGET(string name, ResolveFieldContext ctx)
        {
            dynamic returnObj = new SimpleJson.JsonObject();
            string url = ResolveUrl(name, ctx);
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(url as string);
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add("X-ClusterKey", "srb6enxednjjeeutjkpq4donu55r7of1");
                webRequest.Headers.Add("X-UserName", "admin");
                using (WebResponse webResponse = await webRequest.GetResponseAsync())
                {
                    // Get the stream associated with the response.
                    Stream receiveStream = webResponse.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                    // convert stream to JsonObject
                    dynamic response = SimpleJson.SimpleJson.DeserializeObject(readStream.ReadToEnd());

                    if (response.ErrorCode != 0) ctx.Errors.Add(new GraphQL.ExecutionError("ErrorCode: " + response.ErrorCode + ". ErrorText: " + response.Data.ErrorText));

                    if (response.Data.items == null) response.Data.items = new List<SimpleJson.JsonObject>();

                    List<dynamic> items = new List<dynamic>();

                    foreach (var item in response.Data.items)
                    {
                        dynamic obj = new SimpleJson.JsonObject();

                        // map each key / value pair to new object with lowercase keys
                        foreach (var entry in item)
                        {
                            obj[entry.Key.ToLower()] = entry.Value;
                        }

                        items.Add(obj);
                    }

                    returnObj.items = items;
                    returnObj.value = items.Count;

                    webResponse.Close();
                    readStream.Close();
                }
            }
            catch (WebException ex)
            {
                ctx.Errors.Add(new GraphQL.ExecutionError(ex.Message.ToString()));
            }
            catch (Exception ex)
            {
                ctx.Errors.Add(new GraphQL.ExecutionError(ex.Message.ToString()));
            }

            return Task.FromResult(returnObj as object).Result;
        }

        // constructs url with parameters if provided
        string ResolveUrl(string name, ResolveFieldContext ctx)
        {
            string url = "";

            switch (name)
            {
                case "issueState":
                    url = "http://localhost:2014/api/adenin.GateKeeper.Connector/briefing/issues?";
                    break;

                case "taskState":
                    url = "http://localhost:2014/api/adenin.GateKeeper.Connector/briefing/tasks?";
                    break;

                case "createIssue":
                    url = "http://localhost:2014/api/adenin.GateKeeper.Connector/poc-connector/issue-create";
                    break;

                case "createTask":
                    url = "http://localhost:2014/api/adenin.GateKeeper.Connector/poc-connector/task-create";
                    break;

                default:
                    break;
            }

            string startDate = "";
            string endDate = "";
            int first = 0;
            int offset = 0;

            if (ctx.HasArgument("startdate") && ctx.HasArgument("enddate"))
            {
                startDate = ctx.GetArgument<string>("startdate");
                endDate = ctx.GetArgument<string>("enddate");
            }

            if (ctx.HasArgument("first")) first = ctx.GetArgument<int>("first");
            if (ctx.HasArgument("offset")) offset = ctx.GetArgument<int>("offset");

            AppendParameter(ref url, "startDate", startDate);
            AppendParameter(ref url, "endDate", endDate);

            int page = 0;
            if (offset > 0) page = offset / first;

            int pageSize = (offset + first) - (page * first);
            int waste = pageSize - first;

            AppendParameter(ref url, "page", page.ToString());
            AppendParameter(ref url, "pageSize", pageSize.ToString());

            return url;
        }

        void AppendParameter(ref string url, string name, string value)
        {
            if (value != "" && value !="0")
            {
                if (!url.EndsWith("?")) url += "&";
                url += name + "=" + value;
            }
        }

        public async Task<dynamic> ExecuteRequestPOST(string name, ResolveFieldContext ctx)
        {
            string title = "";
            string description = "";
            string date = "";

            if (ctx.HasArgument("title")) title = ctx.GetArgument<string>("title");
            if (ctx.HasArgument("description")) description = ctx.GetArgument<string>("description");
            if (ctx.HasArgument("date")) date = ctx.GetArgument<string>("date");

            dynamic item = new SimpleJson.JsonObject();
            item.id = Guid.NewGuid().ToString();
            item.title = title;
            item.description = description;
            item.date = date;

            // send post request to gitlab connector to create Issue
            try
            {
                string url = ResolveUrl(name, ctx);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add("X-ClusterKey", "srb6enxednjjeeutjkpq4donu55r7of1");
                webRequest.Headers.Add("X-UserName", "admin");

                string stringData = SimpleJson.SimpleJson.SerializeObject(item); //place body here
                var data = Encoding.ASCII.GetBytes(stringData); // or UTF8
                webRequest.ContentLength = data.Length;
                var newStream = webRequest.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                using (HttpWebResponse response = await webRequest.GetResponseAsync() as HttpWebResponse)
                {

                    // Get the stream associated with the response.
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                    // convert stream to JsonObject
                    dynamic res = SimpleJson.SimpleJson.DeserializeObject(readStream.ReadToEnd());

                    if (res.ErrorCode != 0) ctx.Errors.Add(new GraphQL.ExecutionError("ErrorCode: " + res.ErrorCode + ". ErrorText: " + res.Data.ErrorText));

                    response.Close();
                    readStream.Close();
                }
            }
            catch (WebException ex)
            {
                ctx.Errors.Add(new GraphQL.ExecutionError(ex.Message.ToString()));
            }
            catch (Exception ex)
            {
                ctx.Errors.Add(new GraphQL.ExecutionError(ex.Message.ToString()));
            }

            return Task.FromResult(item as object).Result;
        }
    }
}
