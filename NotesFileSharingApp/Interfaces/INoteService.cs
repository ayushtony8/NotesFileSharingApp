using NotesFileSharingApp.DTOs;

namespace NotesFileSharingApp.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<NoteDto>> GetAllNotesAsync(string userId);
        Task<NoteDto?> GetNoteByIdAsync(int id, string userId);
        Task<NoteDto> CreateNoteAsync(CreateNoteDto createNoteDto, string userId);
        Task<NoteDto?> UpdateNoteAsync(UpdateNoteDto updateNoteDto, string userId);
        Task<bool> DeleteNoteAsync(int id, string userId);
        Task<IEnumerable<NoteDto>> SearchNotesAsync(string searchTerm, string userId);
        Task<bool> UserCanAccessNoteAsync(int noteId, string userId);
    }
}