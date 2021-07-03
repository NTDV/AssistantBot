using System.ComponentModel.DataAnnotations;

namespace PersonalBot.Data.Models
{
    public abstract class DbItem
    {
        [Key, Required] 
        public long Id { get; set; }
        [Required] 
        public long ChatId { get; set; }

        public abstract bool IsContentEquals(DbItem item);
    }
}