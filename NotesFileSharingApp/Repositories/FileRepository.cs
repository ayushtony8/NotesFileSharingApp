using Microsoft.EntityFrameworkCore;
using NotesFileSharingApp.Data;
using NotesFileSharingApp.Interfaces;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;

        public FileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FileUpload>> GetAllAsync()
        {
            return await _context.Files
                .Include(f => f.User)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<FileUpload>> GetByUserIdAsync(string userId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId)
                .Include(f => f.User)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task<FileUpload?> GetByIdAsync(int id)
        {
            return await _context.Files
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FileUpload?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Files
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

        public async Task<FileUpload> CreateAsync(FileUpload file)
        {
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null)
                return false;

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Files.AnyAsync(f => f.Id == id);
        }

        public async Task<bool> UserOwnsFileAsync(int fileId, string userId)
        {
            return await _context.Files.AnyAsync(f => f.Id == fileId && f.UserId == userId);
        }

        public async Task<IEnumerable<FileUpload>> GetByFileTypeAsync(string fileType, string userId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId && f.FileType.Contains(fileType))
                .Include(f => f.User)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }
    }
}