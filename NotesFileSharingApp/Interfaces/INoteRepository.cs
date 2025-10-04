using NotesFileSharingApp.DTOs;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllAsync();
        Task<IEnumerable<Note>> GetByUserIdAsync(string userId);
        Task<Note?> GetByIdAsync(int id);
        Task<Note?> GetByIdAndUserIdAsync(int id, string userId);
        Task<Note> CreateAsync(Note note);
        Task<Note> UpdateAsync(Note note);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Note>> SearchAsync(string searchTerm, string userId);
        Task<bool> ExistsAsync(int id);
        Task<bool> UserOwnsNoteAsync(int noteId, string userId);
    }
}