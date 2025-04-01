using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Identity.Models
{
    [Table("Comments")]
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required]
        public int? StockId { get; set; }
        public Stock? Stock { get; set; }
        [Required]
        public string AppUserId { get; set; } // Foreign Key for AppUser
        public AppUser AppUser { get; set; } // Navigation Property for AppUser. Establishong the one 2 one relationship
    }
}