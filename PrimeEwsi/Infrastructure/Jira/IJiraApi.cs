﻿using System.Collections.Generic;
using PrimeEwsi.Models;

namespace PrimeEwsi.Infrastructure.Jira
{
    public interface IJiraApi
    {
        IEnumerable<JiraTeet> GetJiraTets(string cookie);
        IEnumerable<string> GetComponents(string cookie);
        IEnumerable<string> GetEnvironment(string cookie);
        IEnumerable<string> GetProjects(string cookie);
    }
}