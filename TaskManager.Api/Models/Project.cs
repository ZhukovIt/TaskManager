using System;
using System.Collections.Generic;

namespace TaskManager.Api.Models
{
    public sealed class Project : CommonObject
    {
        public List<User> AllUsers { get; set; }

        public List<Desk> AllDesks { get; set; }
    }
}