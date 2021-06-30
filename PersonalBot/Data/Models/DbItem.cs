using System.ComponentModel.DataAnnotations;

namespace PersonalBot.Data.Models
{
    public class DbItem
    {
        [Key, Required] 
        public long Id { get; set; }
        [Required] 
        public long ChatId { get; set; }
    }
}