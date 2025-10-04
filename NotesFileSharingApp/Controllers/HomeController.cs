using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesFileSharingApp.Interfaces;
using NotesFileSharingApp.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace NotesFileSharingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INoteService _noteService;
        private readonly IFileService _fileService;
        private readonly ISharedService _sharedService;

        public HomeController(ILogger<HomeController> logger, INoteService noteService, 
            IFileService fileService, ISharedService sharedService)
        {
            _logger = logger;
            _noteService = noteService;
            _fileService = fileService;
            _sharedService = sharedService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    // Get dashboard data
                    var myNotes = await _noteService.GetAllNotesAsync(userId);
                    var myFiles = await _fileService.GetAllFilesAsync(userId);
                    var sharedNotesWithMe = await _sharedService.GetNotesSharedWithMeAsync(userId);
                    var sharedFilesWithMe = await _sharedService.GetFilesSharedWithMeAsync(userId);

                    ViewBag.MyNotesCount = myNotes.Count();
                    ViewBag.MyFilesCount = myFiles.Count();
                    ViewBag.SharedNotesCount = sharedNotesWithMe.Count();
                    ViewBag.SharedFilesCount = sharedFilesWithMe.Count();
                    
                    // Recent notes (last 5)
                    ViewBag.RecentNotes = myNotes.Take(5);
                    
                    // Recent files (last 5)
                    ViewBag.RecentFiles = myFiles.Take(5);
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
