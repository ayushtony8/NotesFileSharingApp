using System.ComponentModel.DataAnnotations;

namespace NotesFileSharingApp.DTOs
{
    public class FileUploadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsShared { get; set; }
    }

    public class UploadFileDto
    {
        [Required(ErrorMessage = "Please select a file")]
        public IFormFile File { get; set; } = null!;
        
        public string Description { get; set; } = string.Empty;
    }
}