using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NotesFileSharingApp.DTOs;
using NotesFileSharingApp.Interfaces;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Services
{
    public class SharedService : ISharedService
    {
        private readonly ISharedRepository _sharedRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IFileRepository _fileRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public SharedService(ISharedRepository sharedRepository, INoteRepository noteRepository,
            IFileRepository fileRepository, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _sharedRepository = sharedRepository;
            _noteRepository = noteRepository;
            _fileRepository = fileRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<bool> ShareNoteAsync(ShareNoteDto shareNoteDto, string sharedByUserId)
        {
            // Check if the note exists and user owns it
            var note = await _noteRepository.GetByIdAndUserIdAsync(shareNoteDto.NoteId, sharedByUserId);
            if (note == null) return false;

            // Find user by email
            var sharedWithUser = await _userManager.FindByEmailAsync(shareNoteDto.SharedWithEmail);
            if (sharedWithUser == null) return false;

            // Don't share with yourself
            if (sharedWithUser.Id == sharedByUserId) return false;

            // Check if already shared
            var alreadyShared = await _sharedRepository.IsNoteSharedWithUserAsync(shareNoteDto.NoteId, sharedWithUser.Id);
            if (alreadyShared) return false;

            var sharedNote = new SharedNote
            {
                NoteId = shareNoteDto.NoteId,
                SharedByUserId = sharedByUserId,
                SharedWithUserId = sharedWithUser.Id,
                CanEdit = shareNoteDto.CanEdit,
                SharedAt = DateTime.UtcNow
            };

            await _sharedRepository.ShareNoteAsync(sharedNote);
            return true;
        }

        public async Task<bool> ShareFileAsync(ShareFileDto shareFileDto, string sharedByUserId)
        {
            // Check if the file exists and user owns it
            var file = await _fileRepository.GetByIdAndUserIdAsync(shareFileDto.FileId, sharedByUserId);
            if (file == null) return false;

            // Find user by email
            var sharedWithUser = await _userManager.FindByEmailAsync(shareFileDto.SharedWithEmail);
            if (sharedWithUser == null) return false;

            // Don't share with yourself
            if (sharedWithUser.Id == sharedByUserId) return false;

            // Check if already shared
            var alreadyShared = await _sharedRepository.IsFileSharedWithUserAsync(shareFileDto.FileId, sharedWithUser.Id);
            if (alreadyShared) return false;

            var sharedFile = new SharedFile
            {
                FileId = shareFileDto.FileId,
                SharedByUserId = sharedByUserId,
                SharedWithUserId = sharedWithUser.Id,
                SharedAt = DateTime.UtcNow
            };

            await _sharedRepository.ShareFileAsync(sharedFile);
            return true;
        }

        public async Task<IEnumerable<SharedNoteDto>> GetNotesSharedWithMeAsync(string userId)
        {
            var sharedNotes = await _sharedRepository.GetSharedNotesWithUserAsync(userId);
            return _mapper.Map<List<SharedNoteDto>>(sharedNotes);
        }

        public async Task<IEnumerable<SharedFileDto>> GetFilesSharedWithMeAsync(string userId)
        {
            var sharedFiles = await _sharedRepository.GetSharedFilesWithUserAsync(userId);
            return _mapper.Map<List<SharedFileDto>>(sharedFiles);
        }

        public async Task<IEnumerable<SharedNoteDto>> GetNotesSharedByMeAsync(string userId)
        {
            var sharedNotes = await _sharedRepository.GetNotesSharedByUserAsync(userId);
            return _mapper.Map<List<SharedNoteDto>>(sharedNotes);
        }

        public async Task<IEnumerable<SharedFileDto>> GetFilesSharedByMeAsync(string userId)
        {
            var sharedFiles = await _sharedRepository.GetFilesSharedByUserAsync(userId);
            return _mapper.Map<List<SharedFileDto>>(sharedFiles);
        }

        public async Task<bool> UnshareNoteAsync(int noteId, string sharedWithEmail, string sharedByUserId)
        {
            // Check if user owns the note
            var userOwnsNote = await _noteRepository.UserOwnsNoteAsync(noteId, sharedByUserId);
            if (!userOwnsNote) return false;

            // Find user by email
            var sharedWithUser = await _userManager.FindByEmailAsync(sharedWithEmail);
            if (sharedWithUser == null) return false;

            return await _sharedRepository.UnshareNoteAsync(noteId, sharedWithUser.Id);
        }

        public async Task<bool> UnshareFileAsync(int fileId, string sharedWithEmail, string sharedByUserId)
        {
            // Check if user owns the file
            var userOwnsFile = await _fileRepository.UserOwnsFileAsync(fileId, sharedByUserId);
            if (!userOwnsFile) return false;

            // Find user by email
            var sharedWithUser = await _userManager.FindByEmailAsync(sharedWithEmail);
            if (sharedWithUser == null) return false;

            return await _sharedRepository.UnshareFileAsync(fileId, sharedWithUser.Id);
        }
    }
}