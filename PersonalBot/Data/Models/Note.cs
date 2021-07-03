using System.ComponentModel.DataAnnotations;

namespace PersonalBot.Data.Models
{
    public class Note : DbItem
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        
        public override bool IsContentEquals(DbItem item)
        {
            return item is Note @new 
                   && Title == @new.Title 
                   && Description == @new.Description;
        }

        public override string ToString()
        {
            return $"{Title}\n{Description}";
        }
    }
}