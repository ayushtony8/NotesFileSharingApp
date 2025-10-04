using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesFileSharingApp.DTOs;
using NotesFileSharingApp.Interfaces;
using System.Security.Claims;

namespace NotesFileSharingApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly INoteService _noteService;
        private readonly ISharedService _sharedService;

        public NotesController(INoteService noteService, ISharedService sharedService)
        {
            _noteService = noteService;
            _sharedService = sharedService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        // GET: Notes
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var userId = GetUserId();
            IEnumerable<NoteDto> notes;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                notes = await _noteService.SearchNotesAsync(searchTerm, userId);
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                notes = await _noteService.GetAllNotesAsync(userId);
            }

            return View(notes);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNoteDto createNoteDto)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                var note = await _noteService.CreateNoteAsync(createNoteDto, userId);
                TempData["SuccessMessage"] = "Note created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(createNoteDto);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);

            if (note == null)
            {
                return NotFound();
            }

            if (!note.CanEdit)
            {
                TempData["ErrorMessage"] = "You don't have permission to edit this note.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var updateNoteDto = new UpdateNoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content
            };

            return View(updateNoteDto);
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateNoteDto updateNoteDto)
        {
            if (id != updateNoteDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                var updatedNote = await _noteService.UpdateNoteAsync(updateNoteDto, userId);

                if (updatedNote == null)
                {
                    TempData["ErrorMessage"] = "Failed to update note. You may not have permission.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Note updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(updateNoteDto);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);

            if (note == null)
            {
                return NotFound();
            }

            if (note.IsShared)
            {
                TempData["ErrorMessage"] = "You can only delete your own notes.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var success = await _noteService.DeleteNoteAsync(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Note deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete note.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Notes/Share/5
        public async Task<IActionResult> Share(int id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);

            if (note == null || note.IsShared)
            {
                return NotFound();
            }

            var shareNoteDto = new ShareNoteDto
            {
                NoteId = id
            };

            ViewBag.NoteTitle = note.Title;
            return View(shareNoteDto);
        }

        // POST: Notes/Share/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(ShareNoteDto shareNoteDto)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                var success = await _sharedService.ShareNoteAsync(shareNoteDto, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Note shared successfully with {shareNoteDto.SharedWithEmail}!";
                    return RedirectToAction(nameof(Details), new { id = shareNoteDto.NoteId });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to share note. User may not exist or note is already shared with this user.");
                }
            }

            var note = await _noteService.GetNoteByIdAsync(shareNoteDto.NoteId, GetUserId());
            ViewBag.NoteTitle = note?.Title ?? "";
            return View(shareNoteDto);
        }
    }
}