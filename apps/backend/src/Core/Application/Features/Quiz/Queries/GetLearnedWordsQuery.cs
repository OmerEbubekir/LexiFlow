using LexiFlow.Application.Common;
using LexiFlow.Application.Features.Words.DTOs;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Quiz.Queries;

public record GetLearnedWordsQuery(Guid UserId) : IRequest<BaseResponse<List<WordListItemDto>>>;

public class GetLearnedWordsQueryHandler : IRequestHandler<GetLearnedWordsQuery, BaseResponse<List<WordListItemDto>>>
{
    private readonly IUserProgressRepository _userProgressRepository;

    public GetLearnedWordsQueryHandler(IUserProgressRepository userProgressRepository)
    {
        _userProgressRepository = userProgressRepository;
    }

    public async Task<BaseResponse<List<WordListItemDto>>> Handle(GetLearnedWordsQuery request, CancellationToken cancellationToken)
    {
        var learnedProgress = await _userProgressRepository.GetLearnedAsync(request.UserId, cancellationToken);
        
        var dtoList = learnedProgress.Select(p => new WordListItemDto(
            p.WordId,
            p.Word.EnglishWord,
            p.Word.TurkishTranslation,
            p.Word.DifficultyLevel,
            p.Word.Category?.Name
        )).ToList();

        return BaseResponse<List<WordListItemDto>>.SuccessResponse(dtoList);
    }
}
