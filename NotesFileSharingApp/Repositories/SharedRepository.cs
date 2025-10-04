using Microsoft.EntityFrameworkCore;
using NotesFileSharingApp.Data;
using NotesFileSharingApp.Interfaces;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Repositories
{
    public class SharedRepository : ISharedRepository
    {
        private readonly ApplicationDbContext _context;

        public SharedRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SharedNote> ShareNoteAsync(SharedNote sharedNote)
        {
            _context.SharedNotes.Add(sharedNote);
            await _context.SaveChangesAsync();
            return sharedNote;
        }

        public async Task<SharedFile> ShareFileAsync(SharedFile sharedFile)
        {
            _context.SharedFiles.Add(sharedFile);
            await _context.SaveChangesAsync();
            return sharedFile;
        }

        public async Task<IEnumerable<SharedNote>> GetSharedNotesWithUserAsync(string userId)
        {
            return await _context.SharedNotes
                .Where(sn => sn.SharedWithUserId == userId)
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedByUser)
                .Include(sn => sn.SharedWithUser)
                .OrderByDescending(sn => sn.SharedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SharedFile>> GetSharedFilesWithUserAsync(string userId)
        {
            return await _context.SharedFiles
                .Where(sf => sf.SharedWithUserId == userId)
                .Include(sf => sf.File)
                .Include(sf => sf.SharedByUser)
                .Include(sf => sf.SharedWithUser)
                .OrderByDescending(sf => sf.SharedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SharedNote>> GetNotesSharedByUserAsync(string userId)
        {
            return await _context.SharedNotes
                .Where(sn => sn.SharedByUserId == userId)
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedByUser)
                .Include(sn => sn.SharedWithUser)
                .OrderByDescending(sn => sn.SharedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SharedFile>> GetFilesSharedByUserAsync(string userId)
        {
            return await _context.SharedFiles
                .Where(sf => sf.SharedByUserId == userId)
                .Include(sf => sf.File)
                .Include(sf => sf.SharedByUser)
                .Include(sf => sf.SharedWithUser)
                .OrderByDescending(sf => sf.SharedAt)
                .ToListAsync();
        }

        public async Task<bool> UnshareNoteAsync(int noteId, string sharedWithUserId)
        {
            var sharedNote = await _context.SharedNotes
                .FirstOrDefaultAsync(sn => sn.NoteId == noteId && sn.SharedWithUserId == sharedWithUserId);

            if (sharedNote == null)
                return false;

            _context.SharedNotes.Remove(sharedNote);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnshareFileAsync(int fileId, string sharedWithUserId)
        {
            var sharedFile = await _context.SharedFiles
                .FirstOrDefaultAsync(sf => sf.FileId == fileId && sf.SharedWithUserId == sharedWithUserId);

            if (sharedFile == null)
                return false;

            _context.SharedFiles.Remove(sharedFile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsNoteSharedWithUserAsync(int noteId, string userId)
        {
            return await _context.SharedNotes
                .AnyAsync(sn => sn.NoteId == noteId && sn.SharedWithUserId == userId);
        }

        public async Task<bool> IsFileSharedWithUserAsync(int fileId, string userId)
        {
            return await _context.SharedFiles
                .AnyAsync(sf => sf.FileId == fileId && sf.SharedWithUserId == userId);
        }

        public async Task<SharedNote?> GetSharedNoteAsync(int noteId, string userId)
        {
            return await _context.SharedNotes
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedByUser)
                .FirstOrDefaultAsync(sn => sn.NoteId == noteId && sn.SharedWithUserId == userId);
        }

        public async Task<SharedFile?> GetSharedFileAsync(int fileId, string userId)
        {
            return await _context.SharedFiles
                .Include(sf => sf.File)
                .Include(sf => sf.SharedByUser)
                .FirstOrDefaultAsync(sf => sf.FileId == fileId && sf.SharedWithUserId == userId);
        }
    }
}