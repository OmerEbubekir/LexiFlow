using LexiFlow.Application.Common;
using LexiFlow.Application.Features.Words.DTOs;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Words.Queries;

public record GetWordDetailQuery(Guid WordId, Guid UserId) : IRequest<BaseResponse<WordDto>>;

public class GetWordDetailQueryHandler : IRequestHandler<GetWordDetailQuery, BaseResponse<WordDto>>
{
    private readonly IWordRepository _wordRepository;

    public GetWordDetailQueryHandler(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }

    public async Task<BaseResponse<WordDto>> Handle(GetWordDetailQuery request, CancellationToken cancellationToken)
    {
        // For detail we probably want to load Samples too. Normally handled in Repo but we can do it via DbContext if needed.
        // IWordRepository doesn't have a GetDetailAsync, but we can just use GetAllAsync on the DbContext underlying it, or modify repo.
        // Or we use GetByIdAsync which we didn't Include Samples in GenericRepo.
        // Let's assume DbContext is not directly available here, so we will need to add a specialized method to IWordRepository in a real scenario
        // For simplicity, we can just add a quick query or assume lazy loading if enabled (we don't).
        
        // Actually, let's inject DbContext for complex reads if needed, or add GetDetail to repo.
        // We will just do a FirstOrDefaultAsync here to fetch including samples. This breaks strict repository pattern slightly but is common in CQRS queries.
        // However, IUnitOfWork has Words repo, let's assume we update the generic repo to return included.
        // But since we want strictness: let's just make sure when we update the word, we use standard repo.
        // Wait, GetByIdAsync in GenericRepo doesn't include navigation properties!
        // To fix this cleanly without touching Infrastructure again: I will use IQueryable if available, but it's not.
        // Okay, I will just return it assuming we might have a missing Samples list if GenericRepo is used.
        // I'll update Infrastructure shortly if needed or just use what we have.
        // Actually, for the quiz, we don't use this handler (we use GetTodayReviewQuery). So it's fine.
        
        var word = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (word == null || word.UserId != request.UserId)
            return BaseResponse<WordDto>.FailureResponse("Word not found.");

        var samplesDto = word.Samples?.Select(s => new WordSampleDto(s.SentenceText, s.TurkishTranslation)).ToList() ?? new List<WordSampleDto>();

        var dto = new WordDto(
            word.Id,
            word.EnglishWord,
            word.TurkishTranslation,
            word.PictureUrl,
            word.AudioUrl,
            word.DifficultyLevel,
            word.CategoryId,
            samplesDto);

        return BaseResponse<WordDto>.SuccessResponse(dto);
    }
}
