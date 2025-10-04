using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesFileSharingApp.Models
{
    public class SharedNote
    {
        [Key]
        public int Id { get; set; }

        public int NoteId { get; set; }

        [Required]
        public string SharedByUserId { get; set; } = string.Empty;

        [Required]
        public string SharedWithUserId { get; set; } = string.Empty;

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;

        public bool CanEdit { get; set; } = false;

        // Navigation Properties
        [ForeignKey("NoteId")]
        public virtual Note Note { get; set; } = null!;

        [ForeignKey("SharedByUserId")]
        public virtual ApplicationUser SharedByUser { get; set; } = null!;

        [ForeignKey("SharedWithUserId")]
        public virtual ApplicationUser SharedWithUser { get; set; } = null!;
    }
}