using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NotesFileSharingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
        public virtual ICollection<FileUpload> Files { get; set; } = new List<FileUpload>();
        public virtual ICollection<SharedNote> SharedNotes { get; set; } = new List<SharedNote>();
        public virtual ICollection<SharedFile> SharedFiles { get; set; } = new List<SharedFile>();
        public virtual ICollection<SharedNote> ReceivedSharedNotes { get; set; } = new List<SharedNote>();
        public virtual ICollection<SharedFile> ReceivedSharedFiles { get; set; } = new List<SharedFile>();
    }
}