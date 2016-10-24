using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrimeEwsi.Infrastructure.Jira
{
    using System.Net;
    using System.Text.RegularExpressions;
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

        public IEnumerable<string> GetComponents(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return new List<string>();
            }

            var restClient = new RestClient("https://godzilla.centrala.bzwbk:9999");

            var request = new RestRequest($"rest/api/2/search?jql=issuetype=\"13\"AND(\"summary\"~\"PRIME*\"OR\"summary\"~\"PIS*\")&fields=summary", Method.GET);

            request.AddCookie("JSESSIONID", cookie);

            var response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new List<string>();
            }

            var jResponse = JObject.Parse(response.Content);

            return jResponse["issues"].Select(m => m["fields"]["summary"].Value<string>()).Where(s=>Regex.IsMatch(s,"PRIME_*") || Regex.IsMatch(s, "PIS_")).OrderBy(s=>s);

        }


        public IEnumerable<string> GetEnvironment(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return new List<string>();
            }

            var restClient = new RestClient("https://godzilla.centrala.bzwbk:9999");

            var request = new RestRequest($"/rest/api/2/search?jql=issuetype=\"15\"&fields=summary", Method.GET);

            request.AddCookie("JSESSIONID", cookie);

            var response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new List<string>();
            }

            var jResponse = JObject.Parse(response.Content);

            return jResponse["issues"].Select(m => m["fields"]["summary"].Value<string>()).Where(s=>s.Contains("ZT")).OrderBy(s=>s);

        }
    }
}