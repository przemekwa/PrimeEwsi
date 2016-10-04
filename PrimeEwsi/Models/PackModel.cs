﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class PackModel : UserModel
    {
        public IEnumerable<Pack> HistoryPackCollection { get; set; }

        public string Teets { get; set; }
      
        public string ProjectId { get; set; }
      
        public List<string> Files { get; set; } = new List<string>();

        public string Component { get; set; }

        public string TestEnvironment { get; set; }

        public PackModel()
        {
            
        }

       
        public PackModel(UserModel userModel)
        {
            InitUser(userModel);
        }

        public void InitUser(UserModel userModel)
        {
            this.Id = userModel.Id;
            this.Name = userModel.Name;
            this.Skp = userModel.Skp;
            this.SvnPassword = userModel.SvnPassword;
            this.SvnUser = userModel.SvnUser;
            this.ApiKey = userModel.ApiKey;
        }
    }
}