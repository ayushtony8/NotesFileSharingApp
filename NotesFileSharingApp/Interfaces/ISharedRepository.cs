using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Interfaces
{
    public interface ISharedRepository
    {
        Task<SharedNote> ShareNoteAsync(SharedNote sharedNote);
        Task<SharedFile> ShareFileAsync(SharedFile sharedFile);
        Task<IEnumerable<SharedNote>> GetSharedNotesWithUserAsync(string userId);
        Task<IEnumerable<SharedFile>> GetSharedFilesWithUserAsync(string userId);
        Task<IEnumerable<SharedNote>> GetNotesSharedByUserAsync(string userId);
        Task<IEnumerable<SharedFile>> GetFilesSharedByUserAsync(string userId);
        Task<bool> UnshareNoteAsync(int noteId, string sharedWithUserId);
        Task<bool> UnshareFileAsync(int fileId, string sharedWithUserId);
        Task<bool> IsNoteSharedWithUserAsync(int noteId, string userId);
        Task<bool> IsFileSharedWithUserAsync(int fileId, string userId);
        Task<SharedNote?> GetSharedNoteAsync(int noteId, string userId);
        Task<SharedFile?> GetSharedFileAsync(int fileId, string userId);
    }
}