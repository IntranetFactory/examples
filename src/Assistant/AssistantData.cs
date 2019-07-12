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
                Title = "Issue1"
            });
            testlist.Add(new issue
            {
                Id = "2",
                Title = "Issue2"
            });
            testlist.Add(new issue
            {
                Id = "3",
                Title = "Issue3"
            });
            testlist.Add(new issue
            {
                Id = "4",
                Title = "Issue4"
            });
            testlist.Add(new issue
            {
                Id = "5",
                Title = "Issue5"
            });
        }

        public Task<List<issue>> GetIssuesFromStaticList()
        {

            return Task.FromResult(testlist);
        }

        public issue AddIssue(issue _issue)
        {
            _issue.Id = Guid.NewGuid().ToString();
            testlist.Add(_issue);
            return _issue;
        }
    }
}
