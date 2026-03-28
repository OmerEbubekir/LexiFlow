using System.Net.Http.Json;
using System.Text.Json;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LexiFlow.Infrastructure.ExternalServices;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GeminiApi:ApiKey"] ?? string.Empty;
        _baseUrl = configuration["GeminiApi:BaseUrl"] ?? "https://generativelanguage.googleapis.com";
    }

    public async Task<string> GenerateStoryAsync(IEnumerable<string> words, string language)
    {
        var wordList = string.Join(", ", words);
        var prompt = $"Create a short, creative story in {language} using the following words strictly: {wordList}. Return ONLY the story text, do not add any markdown formatting or explanations.";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var requestUri = $"{_baseUrl.TrimEnd('/')}/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
        
        var response = await _httpClient.PostAsJsonAsync(requestUri, requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"AI Generation Failed: {response.StatusCode}. Details: {err}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseJson);
        
        // Extract the text part from the deeply nested json
        var text = jsonDoc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text ?? string.Empty;
    }

    public async Task<string> GenerateImageAsync(string storyText)
    {
        // Note: As warned, gemini-1.5-flash cannot generate images natively. 
        // This is a placeholder for an Imagen/Stability API endpoint.
        
        await Task.Delay(500); // Simulate network latency
        
        return "https://placehold.co/600x400/png?text=AI+Generated+Image+Placeholder";
    }
}
