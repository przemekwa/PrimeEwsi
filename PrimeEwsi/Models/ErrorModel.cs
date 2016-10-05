﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class ErrorModel : UserModel
    {
        public string Description { get; set; }

        public ErrorModel(string description)
        {
            Description = description;
        }
    }
}