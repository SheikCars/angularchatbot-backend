namespace ChatbotApi.Models
{
    public class ChatMessage
    {
        public long Id { get; set; } // Primary key with Identity
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

}