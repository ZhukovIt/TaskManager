using System;

namespace TaskManager.Api.Models
{
    public sealed class Desk : CommonObject
    {
        public bool IsPrivate { get; set; }

        public string Columns { get; set; }

        public User Admin { get; set; }
    }
}