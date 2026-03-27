namespace LexiFlow.Domain.Interfaces;

public interface IGeminiService
{
    Task<string> GenerateStoryAsync(IEnumerable<string> words, string language);
    Task<string> GenerateImageAsync(string storyText);
}
