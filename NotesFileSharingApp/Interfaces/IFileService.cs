using NotesFileSharingApp.DTOs;

namespace NotesFileSharingApp.Interfaces
{
    public interface IFileService
    {
        Task<IEnumerable<FileUploadDto>> GetAllFilesAsync(string userId);
        Task<FileUploadDto?> GetFileByIdAsync(int id, string userId);
        Task<FileUploadDto> UploadFileAsync(UploadFileDto uploadFileDto, string userId);
        Task<bool> DeleteFileAsync(int id, string userId);
        Task<IEnumerable<FileUploadDto>> GetFilesByTypeAsync(string fileType, string userId);
        Task<bool> UserCanAccessFileAsync(int fileId, string userId);
        Task<string> GetFilePathAsync(int fileId, string userId);
    }
}