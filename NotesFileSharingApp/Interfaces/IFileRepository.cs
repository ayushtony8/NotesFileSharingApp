using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<FileUpload>> GetAllAsync();
        Task<IEnumerable<FileUpload>> GetByUserIdAsync(string userId);
        Task<FileUpload?> GetByIdAsync(int id);
        Task<FileUpload?> GetByIdAndUserIdAsync(int id, string userId);
        Task<FileUpload> CreateAsync(FileUpload file);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> UserOwnsFileAsync(int fileId, string userId);
        Task<IEnumerable<FileUpload>> GetByFileTypeAsync(string fileType, string userId);
    }
}