using System;

namespace TaskManager.Api.Models
{
    public sealed class Task : CommonObject
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte[] File { get; set; }
    }
}