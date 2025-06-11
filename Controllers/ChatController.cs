using ChatbotApi.Data;
using ChatbotApi.Models;
using ChatbotApi.Services; // <-- Add this
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatbotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatContext _context;
        private readonly INlpService _nlpService; // <-- Inject the service

        public ChatController(ChatContext context, INlpService nlpService)
        {
            _context = context;
            _nlpService = nlpService; // <-- Assign it
        }

        // GET: api/chat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatMessages()
        {
            return await _context.ChatMessages.OrderBy(m => m.Timestamp).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ChatMessage>> PostChatMessage(ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            // 🚫 Prevent inserting a manually set ID
            message.Id = 0;

            message.User = "User";
            message.Timestamp = DateTime.UtcNow;
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            var botResponseText = await _nlpService.GenerateResponse(message.Message);

            var botResponse = new ChatMessage
            {
                User = "Bot",
                Message = botResponseText,
                Timestamp = DateTime.UtcNow.AddSeconds(1)
            };
            _context.ChatMessages.Add(botResponse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChatMessages), new { id = message.Id }, message);
        }
    }
}