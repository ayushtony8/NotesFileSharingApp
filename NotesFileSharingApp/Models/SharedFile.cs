using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesFileSharingApp.Models
{
    public class SharedFile
    {
        [Key]
        public int Id { get; set; }

        public int FileId { get; set; }

        [Required]
        public string SharedByUserId { get; set; } = string.Empty;

        [Required]
        public string SharedWithUserId { get; set; } = string.Empty;

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("FileId")]
        public virtual FileUpload File { get; set; } = null!;

        [ForeignKey("SharedByUserId")]
        public virtual ApplicationUser SharedByUser { get; set; } = null!;

        [ForeignKey("SharedWithUserId")]
        public virtual ApplicationUser SharedWithUser { get; set; } = null!;
    }
}