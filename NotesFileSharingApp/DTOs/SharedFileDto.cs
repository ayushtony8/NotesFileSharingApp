using System.ComponentModel.DataAnnotations;

namespace NotesFileSharingApp.DTOs
{
    public class SharedFileDto
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string SharedByUserId { get; set; } = string.Empty;
        public string SharedByUserName { get; set; } = string.Empty;
        public string SharedWithUserId { get; set; } = string.Empty;
        public string SharedWithUserName { get; set; } = string.Empty;
        public DateTime SharedAt { get; set; }
    }

    public class ShareFileDto
    {
        public int FileId { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string SharedWithEmail { get; set; } = string.Empty;
    }
}