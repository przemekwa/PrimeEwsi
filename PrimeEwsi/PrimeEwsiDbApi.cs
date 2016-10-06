﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrimeEwsi.Models;

namespace PrimeEwsi
{
    public class PrimeEwsiDbApi
    {
        public PrimeEwsiContext PrimeEwsiContext { get; set; }

        public PrimeEwsiDbApi(PrimeEwsiContext primeEwsiContext)
        {
            PrimeEwsiContext = new PrimeEwsiContext();
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

            return this.PrimeEwsiContext.SaveChanges();
        }

        public int AddUser(UserModel userModel)
        {
            this.PrimeEwsiContext.UsersModel.Add(userModel);

            return this.PrimeEwsiContext.SaveChanges();
        }


       
    }
}