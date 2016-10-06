
using System;

namespace PrimeEwsi.Infrastructure
{
    using System.Linq;
    using System.Web;
    using Newtonsoft.Json;
    using PrimeEwsi.Models;

    public enum Version
    {
        Major,
        Minor,
        Bug,
        Build
    };

    public static class Helper
    {
        public static UserModel GetUserModel()
        {
            var userSkp = HttpContext.Current.User.Identity.Name;
            
            var primeEwsiContext = new PrimeEwsiContext();

            return primeEwsiContext.UsersModel.SingleOrDefault(m => m.UserSkp == userSkp);
        }

        public static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        public static string UpdateVersion(string currentVersion, Version version = Version.Build)
        {
            if (string.IsNullOrEmpty(currentVersion))
            {
                return currentVersion;
            }

            var versionArray = currentVersion.Split('.');

            switch (version)
            {
                    case Version.Build:

                    var intNumber = int.Parse(versionArray[3]);

                    intNumber++;

                    versionArray[3] = intNumber.ToString();
             
                    break;
            }

            return string.Join(".", versionArray);
        }

        
    }
}