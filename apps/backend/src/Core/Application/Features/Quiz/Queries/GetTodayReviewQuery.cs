using LexiFlow.Application.Common;
using LexiFlow.Application.Features.Quiz.DTOs;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Quiz.Queries;

public record GetTodayReviewQuery(Guid UserId) : IRequest<BaseResponse<List<QuizItemDto>>>;

public class GetTodayReviewQueryHandler : IRequestHandler<GetTodayReviewQuery, BaseResponse<List<QuizItemDto>>>
{
    private readonly IUserProgressRepository _userProgressRepository;
    private readonly IUserRepository _userRepository;

    public GetTodayReviewQueryHandler(IUserProgressRepository userProgressRepository, IUserRepository userRepository)
    {
        _userProgressRepository = userProgressRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<List<QuizItemDto>>> Handle(GetTodayReviewQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<List<QuizItemDto>>.FailureResponse("User not found.");

        // 1. Get due for review (Words where NextReviewDate is past or today, and not learned)
        var dueWords = await _userProgressRepository.GetDueForReviewAsync(request.UserId, cancellationToken);

        // 2. Add New Words up to the DailyNewWordLimit limit
        var newWordsCount = dueWords.Count(p => p.Stage == Domain.Enums.RepetitionStage.New);
        var remainingNewQuota = user.DailyNewWordLimit - newWordsCount;

        var result = dueWords.ToList();

        if (remainingNewQuota > 0)
        {
            var newWords = await _userProgressRepository.GetNewWordsAsync(request.UserId, remainingNewQuota, cancellationToken);
            // Merge unique words (ensure we don't accidentally pull duplicates if they were somehow in dueWords)
            foreach (var nw in newWords)
            {
                if (!result.Any(r => r.WordId == nw.WordId))
                {
                    result.Add(nw);
                }
            }
        }

        var dtoList = result.Select(p => new QuizItemDto(
            p.WordId,
            p.Word.EnglishWord,
            p.Word.TurkishTranslation,
            p.Word.PictureUrl,
            p.Word.AudioUrl,
            p.Stage,
            p.Word.Samples?.Select(s => new QuizSampleDto(s.SentenceText, s.TurkishTranslation)).ToList() ?? new List<QuizSampleDto>()
        )).ToList();

        return BaseResponse<List<QuizItemDto>>.SuccessResponse(dtoList);
    }
}
