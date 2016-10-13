using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrimeEwsi.Infrastructure.Jira
{
    using System.Net;
    using Models;
    using Newtonsoft.Json.Linq;
    using RestSharp;

    public class JiraApi : IJiraApi
    {
        public IEnumerable<JiraTeet> GetJiraTets(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return new List<JiraTeet>();
            }

            var restClient = new RestClient("https://godzilla.centrala.bzwbk:9999");

            var request = new RestRequest($"rest/api/2/search?jql=AssigneeGroup='!DRA-ZRP'&fields=id,key,summary", Method.GET);

            request.AddCookie("JSESSIONID", cookie);

            var response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new List<JiraTeet>();
            }

            var jResponse = JObject.Parse(response.Content);

            return jResponse["issues"]
                .Where(i => i["key"].Value<string>().Contains("TEET"))
                .Select(i => new JiraTeet
                {
                    Id = i["key"].Value<string>(),
                    Summary = i["fields"]["summary"].Value<string>()
                }).Take(10);
        }
    }
}