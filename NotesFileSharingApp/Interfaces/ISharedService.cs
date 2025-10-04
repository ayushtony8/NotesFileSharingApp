using NotesFileSharingApp.DTOs;

namespace NotesFileSharingApp.Interfaces
{
    public interface ISharedService
    {
        Task<bool> ShareNoteAsync(ShareNoteDto shareNoteDto, string sharedByUserId);
        Task<bool> ShareFileAsync(ShareFileDto shareFileDto, string sharedByUserId);
        Task<IEnumerable<SharedNoteDto>> GetNotesSharedWithMeAsync(string userId);
        Task<IEnumerable<SharedFileDto>> GetFilesSharedWithMeAsync(string userId);
        Task<IEnumerable<SharedNoteDto>> GetNotesSharedByMeAsync(string userId);
        Task<IEnumerable<SharedFileDto>> GetFilesSharedByMeAsync(string userId);
        Task<bool> UnshareNoteAsync(int noteId, string sharedWithEmail, string sharedByUserId);
        Task<bool> UnshareFileAsync(int fileId, string sharedWithEmail, string sharedByUserId);
    }
}