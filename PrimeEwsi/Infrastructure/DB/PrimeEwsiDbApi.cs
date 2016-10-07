using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrimeEwsi.Models;

namespace PrimeEwsi
{
    public class PrimeEwsiDbApi : IPrimeEwsiDbApi
    {
        public PrimeEwsiContext PrimeEwsiContext { get; set; }

        public PrimeEwsiDbApi(PrimeEwsiContext primeEwsiContext)
        {
            PrimeEwsiContext = primeEwsiContext;
        }

        public HistoryPackModel GetHistoryPacksByPackId(int packId)
        {
            return this.PrimeEwsiContext.HistoryPackColection.SingleOrDefault(p => p.Id == packId);
        }

        public IEnumerable<HistoryPackModel> GetHistoryPacksByUserId(int userId)
        {
            return this.PrimeEwsiContext.HistoryPackColection.Where(p => p.UserId == userId);
        }

        public UserModel GetUser(string userName)
        {
            return this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.UserSkp == userName);
        }

        public UserModel GetUser(int userId)
        {
            return this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.UserId == userId);
        }

        public int UpdateUser(UserModel userModel)
        {
            var user = this.GetUser(userModel.UserId);

            user.UserName = userModel.UserName;
            user.UserApiKey = userModel.UserApiKey;
            user.SvnUser = userModel.SvnUser;
            user.SvnPassword = userModel.SvnPassword;
            user.UserJiraCookie = userModel.UserJiraCookie;

            return this.PrimeEwsiContext.SaveChanges();
        }

        public int AddUser(UserModel userModel)
        {
            this.PrimeEwsiContext.UsersModel.Add(userModel);

            return this.PrimeEwsiContext.SaveChanges();
        }


        public ConfigModel GetConfigModelByComponent(string component)
        {
            return this.PrimeEwsiContext.ConfigModel.Single(c => c.Component == component);
        }

        public int AddHistoryPack(HistoryPackModel historyPackModel)
        {
            this.PrimeEwsiContext.HistoryPackColection.Add(historyPackModel);

            return this.PrimeEwsiContext.SaveChanges();
        }

        public int UpdateConfig(ConfigModel configModel)
        {
            var config = this.GetConfigModelByComponent(configModel.Component);

            config.Version = configModel.Version;

            return this.PrimeEwsiContext.SaveChanges();
        }

       
    }
}