namespace ChatbotApi.Services
{
    public interface INlpService
    {
        Task<string> GenerateResponse(string prompt);
    }
}