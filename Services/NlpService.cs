using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatbotApi.Services
{
    public class NlpService : INlpService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NlpService> _logger;

        public NlpService(HttpClient httpClient, IConfiguration configuration, ILogger<NlpService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateResponse(string prompt)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Gemini API key is not configured in appsettings.json.");
                return "I can't respond right now. My brain (API key) is missing!";
            }

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API call failed with status code {StatusCode}: {Response}", response.StatusCode, errorContent);
                    return "Sorry, I encountered an error while thinking.";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);

                string? textResponse = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                return textResponse ?? "Sorry, I couldn't come up with a response.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while calling the Gemini API.");
                return "Sorry, a technical glitch is preventing me from responding.";
            }
        }
    }

    // Helper classes for deserializing the Gemini API response
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate>? Candidates { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part>? Parts { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

}