using System.ComponentModel.DataAnnotations;

namespace NotesFileSharingApp.DTOs
{
    public class SharedNoteDto
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string NoteTitle { get; set; } = string.Empty;
        public string NoteContent { get; set; } = string.Empty;
        public string SharedByUserId { get; set; } = string.Empty;
        public string SharedByUserName { get; set; } = string.Empty;
        public string SharedWithUserId { get; set; } = string.Empty;
        public string SharedWithUserName { get; set; } = string.Empty;
        public DateTime SharedAt { get; set; }
        public bool CanEdit { get; set; }
    }

    public class ShareNoteDto
    {
        public int NoteId { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string SharedWithEmail { get; set; } = string.Empty;
        
        public bool CanEdit { get; set; } = false;
    }
}