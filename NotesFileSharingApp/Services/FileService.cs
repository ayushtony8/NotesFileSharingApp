using AutoMapper;
using NotesFileSharingApp.DTOs;
using NotesFileSharingApp.Interfaces;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ISharedRepository _sharedRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IFileRepository fileRepository, ISharedRepository sharedRepository, 
            IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _fileRepository = fileRepository;
            _sharedRepository = sharedRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<FileUploadDto>> GetAllFilesAsync(string userId)
        {
            var files = await _fileRepository.GetByUserIdAsync(userId);
            var fileDtos = _mapper.Map<List<FileUploadDto>>(files);

            foreach (var fileDto in fileDtos)
            {
                fileDto.IsShared = false; // User's own files
            }

            return fileDtos;
        }

        public async Task<FileUploadDto?> GetFileByIdAsync(int id, string userId)
        {
            var file = await _fileRepository.GetByIdAsync(id);
            if (file == null) return null;

            // Check if user owns the file or has access through sharing
            if (file.UserId == userId)
            {
                var fileDto = _mapper.Map<FileUploadDto>(file);
                fileDto.IsShared = false;
                return fileDto;
            }

            // Check if file is shared with user
            var isShared = await _sharedRepository.IsFileSharedWithUserAsync(id, userId);
            if (isShared)
            {
                var fileDto = _mapper.Map<FileUploadDto>(file);
                fileDto.IsShared = true;
                return fileDto;
            }

            return null;
        }

        public async Task<FileUploadDto> UploadFileAsync(UploadFileDto uploadFileDto, string userId)
        {
            var file = uploadFileDto.File;
            
            // Create uploads directory if it doesn't exist
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file to disk
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Create file entity
            var fileUpload = new FileUpload
            {
                FileName = file.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                FileType = file.ContentType,
                FileSize = file.Length,
                UserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            var createdFile = await _fileRepository.CreateAsync(fileUpload);
            var fileDto = _mapper.Map<FileUploadDto>(createdFile);
            fileDto.IsShared = false;
            
            return fileDto;
        }

        public async Task<bool> DeleteFileAsync(int id, string userId)
        {
            var canAccess = await UserCanAccessFileAsync(id, userId);
            if (!canAccess) return false;

            var userOwns = await _fileRepository.UserOwnsFileAsync(id, userId);
            if (!userOwns) return false; // Only owner can delete

            // Get file to delete from disk
            var file = await _fileRepository.GetByIdAsync(id);
            if (file != null)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, file.FilePath.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            return await _fileRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<FileUploadDto>> GetFilesByTypeAsync(string fileType, string userId)
        {
            var files = await _fileRepository.GetByFileTypeAsync(fileType, userId);
            var fileDtos = _mapper.Map<List<FileUploadDto>>(files);

            foreach (var fileDto in fileDtos)
            {
                fileDto.IsShared = false;
            }

            return fileDtos;
        }

        public async Task<bool> UserCanAccessFileAsync(int fileId, string userId)
        {
            // Check if user owns the file
            if (await _fileRepository.UserOwnsFileAsync(fileId, userId))
                return true;

            // Check if file is shared with user
            return await _sharedRepository.IsFileSharedWithUserAsync(fileId, userId);
        }

        public async Task<string> GetFilePathAsync(int fileId, string userId)
        {
            var canAccess = await UserCanAccessFileAsync(fileId, userId);
            if (!canAccess) return string.Empty;

            var file = await _fileRepository.GetByIdAsync(fileId);
            return file?.FilePath ?? string.Empty;
        }
    }
}