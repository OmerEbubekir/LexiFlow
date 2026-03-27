using LexiFlow.Domain.Enums;

namespace LexiFlow.Application.Features.Quiz.DTOs;

public record QuizSampleDto(string SentenceText, string? TurkishTranslation);

public record QuizItemDto(
    Guid WordId,
    string EnglishWord,
    string TurkishTranslation,
    string? PictureUrl,
    string? AudioUrl,
    RepetitionStage Stage,
    List<QuizSampleDto> Samples);
