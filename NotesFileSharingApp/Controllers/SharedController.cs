using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesFileSharingApp.Interfaces;
using System.Security.Claims;

namespace NotesFileSharingApp.Controllers
{
    [Authorize]
    public class SharingController : Controller
    {
        private readonly ISharedService _sharedService;
        private readonly INoteService _noteService;
        private readonly IFileService _fileService;

        public SharingController(ISharedService sharedService, INoteService noteService, IFileService fileService)
        {
            _sharedService = sharedService;
            _noteService = noteService;
            _fileService = fileService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        // GET: Sharing/Index - Shows content shared with me
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var sharedNotes = await _sharedService.GetNotesSharedWithMeAsync(userId);
            var sharedFiles = await _sharedService.GetFilesSharedWithMeAsync(userId);

            ViewBag.SharedNotes = sharedNotes;
            ViewBag.SharedFiles = sharedFiles;

            return View();
        }

        // GET: Sharing/SharedByMe - Shows content I've shared with others
        public async Task<IActionResult> SharedByMe()
        {
            var userId = GetUserId();
            var sharedNotes = await _sharedService.GetNotesSharedByMeAsync(userId);
            var sharedFiles = await _sharedService.GetFilesSharedByMeAsync(userId);

            ViewBag.SharedNotes = sharedNotes;
            ViewBag.SharedFiles = sharedFiles;

            return View();
        }

        // GET: Sharing/ViewNote/5 - View a shared note
        public async Task<IActionResult> ViewNote(int id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Sharing/DownloadFile/5 - Download a shared file
        public async Task<IActionResult> DownloadFile(int id)
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

        // POST: Sharing/UnshareNote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnshareNote(int noteId, string sharedWithEmail)
        {
            var userId = GetUserId();
            var success = await _sharedService.UnshareNoteAsync(noteId, sharedWithEmail, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Note unshared successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unshare note.";
            }

            return RedirectToAction(nameof(SharedByMe));
        }

        // POST: Sharing/UnshareFile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnshareFile(int fileId, string sharedWithEmail)
        {
            var userId = GetUserId();
            var success = await _sharedService.UnshareFileAsync(fileId, sharedWithEmail, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "File unshared successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unshare file.";
            }

            return RedirectToAction(nameof(SharedByMe));
        }
    }
}