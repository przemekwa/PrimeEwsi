﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrimeEwsi.Models
{
    [Table("users_profile")]
    public class UserModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("skp")]
        public string Skp { get; set; }

        [Column("svn_user")]
        public string SvnUser { get; set; }

        [Column("svn_password")]
        [DataType(DataType.Password)]
        public string SvnPassword { get; set; }

        [Column("apikey")]
        public string ApiKey { get; set; }
    }
}