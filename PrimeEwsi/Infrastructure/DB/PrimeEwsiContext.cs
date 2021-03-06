﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using PrimeEwsi.Models;

namespace PrimeEwsi
{
    public class PrimeEwsiContext : DbContext
    {
        public DbSet<UserModel> UsersModel { get; set; }
        public DbSet<ConfigModel> ConfigModel { get; set; }
        public DbSet<HistoryPackModel> HistoryPackColection { get; set; }

        public PrimeEwsiContext() : base("primeEwsi")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}