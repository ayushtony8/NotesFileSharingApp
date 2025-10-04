using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesFileSharingApp.DTOs;
using NotesFileSharingApp.Interfaces;
using System.Security.Claims;

namespace NotesFileSharingApp.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ISharedService _sharedService;

        public FilesController(IFileService fileService, ISharedService sharedService)
        {
            _fileService = fileService;
            _sharedService = sharedService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        // GET: Files
        public async Task<IActionResult> Index(string fileType = "")
        {
            var userId = GetUserId();
            IEnumerable<FileUploadDto> files;

            if (!string.IsNullOrWhiteSpace(fileType))
            {
                files = await _fileService.GetFilesByTypeAsync(fileType, userId);
                ViewBag.FileType = fileType;
            }
            else
            {
                files = await _fileService.GetAllFilesAsync(userId);
            }

            return View(files);
        }

        // GET: Files/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Files/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadFileDto uploadFileDto)
        {
            if (ModelState.IsValid)
            {
                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(uploadFileDto.File.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("File", "Only PDF, DOC, DOCX, TXT, JPG, JPEG, PNG, and GIF files are allowed.");
                    return View(uploadFileDto);
                }

                // Validate file size (10MB max)
                if (uploadFileDto.File.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("File", "File size cannot exceed 10MB.");
                    return View(uploadFileDto);
                }

                try
                {
                    var userId = GetUserId();
                    var file = await _fileService.UploadFileAsync(uploadFileDto, userId);
                    TempData["SuccessMessage"] = "File uploaded successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error uploading file: {ex.Message}");
                }
            }
            return View(uploadFileDto);
        }

        // GET: Files/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var userId = GetUserId();
            var filePath = await _fileService.GetFilePathAsync(id, userId);

            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound();
            }

            var file = await _fileService.GetFileByIdAsync(id, userId);
            if (file == null)
            {
                return NotFound();
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
            {
                TempData["ErrorMessage"] = "File not found on server.";
                return RedirectToAction(nameof(Index));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, file.FileType, file.FileName);
        }

        // GET: Files/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var file = await _fileService.GetFileByIdAsync(id, userId);

            if (file == null)
            {
                return NotFound();
            }

            if (file.IsShared)
            {
                TempData["ErrorMessage"] = "You can only delete your own files.";
                return RedirectToAction(nameof(Index));
            }

            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var success = await _fileService.DeleteFileAsync(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "File deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete file.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Files/Share/5
        public async Task<IActionResult> Share(int id)
        {
            var userId = GetUserId();
            var file = await _fileService.GetFileByIdAsync(id, userId);

            if (file == null || file.IsShared)
            {
                return NotFound();
            }

            var shareFileDto = new ShareFileDto
            {
                FileId = id
            };

            ViewBag.FileName = file.FileName;
            return View(shareFileDto);
        }

        // POST: Files/Share/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(ShareFileDto shareFileDto)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                var success = await _sharedService.ShareFileAsync(shareFileDto, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = $"File shared successfully with {shareFileDto.SharedWithEmail}!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to share file. User may not exist or file is already shared with this user.");
                }
            }

            var file = await _fileService.GetFileByIdAsync(shareFileDto.FileId, GetUserId());
            ViewBag.FileName = file?.FileName ?? "";
            return View(shareFileDto);
        }
    }
}