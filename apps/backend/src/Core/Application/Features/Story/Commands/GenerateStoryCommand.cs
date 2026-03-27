using System.Text.Json;
using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Story.Commands;

public record GenerateStoryResponseDto(Guid StoryId, string StoryText, string? ImagePath);

public record GenerateStoryCommand(Guid UserId, List<Guid> WordIds, string Language) : IRequest<BaseResponse<GenerateStoryResponseDto>>;

public class GenerateStoryCommandValidator : AbstractValidator<GenerateStoryCommand>
{
    public GenerateStoryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WordIds).NotEmpty().Must(x => x.Count == 5).WithMessage("Exactly 5 words must be provided to generate a story.");
        RuleFor(x => x.Language).NotEmpty();
    }
}

public class GenerateStoryCommandHandler : IRequestHandler<GenerateStoryCommand, BaseResponse<GenerateStoryResponseDto>>
{
    private readonly IWordRepository _wordRepository;
    private readonly IGeminiService _geminiService;
    private readonly IRepository<GeneratedStory> _storyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateStoryCommandHandler(
        IWordRepository wordRepository,
        IGeminiService geminiService,
        IRepository<GeneratedStory> storyRepository,
        IUnitOfWork unitOfWork)
    {
        _wordRepository = wordRepository;
        _geminiService = geminiService;
        _storyRepository = storyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<GenerateStoryResponseDto>> Handle(GenerateStoryCommand request, CancellationToken cancellationToken)
    {
        var words = new List<string>();

        foreach (var wordId in request.WordIds)
        {
            var word = await _wordRepository.GetByIdAsync(wordId, cancellationToken);
            if (word == null || word.UserId != request.UserId)
                return BaseResponse<GenerateStoryResponseDto>.FailureResponse($"Word {wordId} is invalid.");
            
            words.Add(word.EnglishWord);
        }

        try
        {
            // 1. Generate Story
            var storyText = await _geminiService.GenerateStoryAsync(words, request.Language);

            // 2. Generate Image (Placeholder)
            var imagePath = await _geminiService.GenerateImageAsync(storyText);

            // 3. Save to DB
            var generatedStory = GeneratedStory.Create(
                request.UserId,
                storyText: storyText,
                wordsUsedJson: JsonSerializer.Serialize(words),
                imagePath: imagePath);

            await _storyRepository.AddAsync(generatedStory, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new GenerateStoryResponseDto(generatedStory.Id, generatedStory.StoryText, generatedStory.ImagePath);
            return BaseResponse<GenerateStoryResponseDto>.SuccessResponse(dto);
        }
        catch (Exception ex)
        {
            return BaseResponse<GenerateStoryResponseDto>.FailureResponse($"AI Generation Failed: {ex.Message}");
        }
    }
}
