using Application.IServices;
using Application.ServiceResponse;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http.Headers;

namespace Application.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
        }

        public async Task<ServiceResponse<string>> CheckSensitiveAsync(string jsonPayload)
        {
            var response = new ServiceResponse<string>();
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                response.Success = false;
                response.Message = "Gemini API key is missing";
                return response;
            }

            try
            {
                var prompt = "Analyze the following JSON for any sensitive or inappropriate content, including nudity, NSFW material, or profanity in any language. Respond with only one word: 'Unqualified' if sensitive content is found, or 'Appropriate' if not. JSON: " + jsonPayload;

                // Minimal Gemini JSON payload shape (Generative Language API - text generation)
                var body = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[] { new { text = prompt } }
                        }
                    }
                };

                var requestContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                using var request = new HttpRequestMessage(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent");
                request.Headers.Add("X-goog-api-key", _apiKey);
                request.Content = requestContent;
                var httpResponse = await _httpClient.SendAsync(request);
                var raw = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    response.Success = false;
                    response.Message = $"Gemini API error: {httpResponse.StatusCode}";
                    return response;
                }

                using var doc = JsonDocument.Parse(raw);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString()?
                    .Trim()
                    .ToLowerInvariant() ?? "Unqualified";

                var answer = (text.Contains("Appropriate", StringComparison.OrdinalIgnoreCase)) ? "Appropriate" : "Unqualified";
                response.Success = true;
                response.Data = answer;
                return response;
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

    }
}


