using ChatbotApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotApi.Data
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}
