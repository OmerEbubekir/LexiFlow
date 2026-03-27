using LexiFlow.Domain.Enums;

namespace LexiFlow.Application.Features.Words.DTOs;

public record WordSampleDto(string SentenceText, string? TurkishTranslation);

public record WordDto(
    Guid Id,
    string EnglishWord,
    string TurkishTranslation,
    string? PictureUrl,
    string? AudioUrl,
    DifficultyLevel DifficultyLevel,
    Guid? CategoryId,
    List<WordSampleDto> Samples);

public record WordListItemDto(
    Guid Id,
    string EnglishWord,
    string TurkishTranslation,
    DifficultyLevel DifficultyLevel,
    string? CategoryName);
