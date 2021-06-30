using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalBot.Data.Models
{
    public class Event : DbItem
    {
        [Required]
        public string Title { get; set; }
        public string Place { get; set; }
        public string Info { get; set; }
        [Required]
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"{Title}\n{Place}\n{Info}";
        }
    }
}