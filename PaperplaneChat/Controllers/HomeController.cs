using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PaperplaneChat.Models;
using PaperplaneChat.Models.VM;
using PaperplaneChat.Services.Interfaces;
using PaperplaneChat.Helpers;

namespace PaperplaneChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IChatService _chatService;

        public HomeController(ILogger<HomeController> logger, IChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            ChatVM chatVM = new ChatVM();
            chatVM.ChatHistory = HttpContext.Session.Get<List<(string, string)>>("Chat");
            if (chatVM.ChatHistory == null)
            {
                chatVM.ChatHistory = new List<(string, string)>();
                HttpContext.Session.Set("Chat", chatVM.ChatHistory);
            }

            chatVM.FirstMessageSent = chatVM.ChatHistory.Count > 0;
            HttpContext.Session.Set("FirstMessageSent", chatVM.FirstMessageSent);

            return View(chatVM);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChatVM chatVM)
        {
            // Retrieve session data
            chatVM.ChatHistory = HttpContext.Session.Get<List<(string, string)>>("Chat");
            if (chatVM.ChatHistory == null)
            {
                chatVM.ChatHistory = new List<(string, string)>();
            }

            // Add user message
            chatVM.ChatHistory.Add(("User", chatVM.UserMessage));

            // Get bot response
            var response = await _chatService.GetChatConversationAsync(chatVM.ChatHistory);
            chatVM.ChatHistory.Add(("Bot", response));

            // Store updated chat history in session
            HttpContext.Session.Set("Chat", chatVM.ChatHistory);
                        
            chatVM.FirstMessageSent = true;
            HttpContext.Session.Set("FirstMessageSent", chatVM.FirstMessageSent);

            return PartialView("_ChatMessages", chatVM);
        }

        public IActionResult ResetChat()
        {
            HttpContext.Session.Remove("Chat");
            HttpContext.Session.Remove("FirstMessageSent");
            return RedirectToAction("Index");
        }

        public IActionResult GenerateImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateImage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Text cannot be empty.");
            }
            var imageUrl = await _chatService.GenerateImageAsync(text);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Failed to generate image.");
            }
            return Json(imageUrl);
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
