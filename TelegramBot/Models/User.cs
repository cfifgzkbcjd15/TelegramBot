using System;
using System.Collections.Generic;

#nullable disable

namespace TelegramBot.Models
{
    public partial class User
    {
        public long TelegramUserId { get; set; }
        public string Faculty { get; set; }
        public string Level { get; set; }
        public string Specialization { get; set; }
        public int Course { get; set; }
    }
}
