using Microsoft.EntityFrameworkCore;
using NotesFileSharingApp.Data;
using NotesFileSharingApp.Interfaces;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly ApplicationDbContext _context;

        public NoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            return await _context.Notes
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Note>> GetByUserIdAsync(string userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId)
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            return await _context.Notes
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Note?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Notes
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        }

        public async Task<Note> CreateAsync(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<Note> UpdateAsync(Note note)
        {
            note.UpdatedAt = DateTime.UtcNow;
            _context.Entry(note).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                return false;

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Note>> SearchAsync(string searchTerm, string userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId && 
                           (n.Title.Contains(searchTerm) || n.Content.Contains(searchTerm)))
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Notes.AnyAsync(n => n.Id == id);
        }

        public async Task<bool> UserOwnsNoteAsync(int noteId, string userId)
        {
            return await _context.Notes.AnyAsync(n => n.Id == noteId && n.UserId == userId);
        }
    }
}