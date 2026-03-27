using LexiFlow.Application.Common;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Analytics.Queries;

public record CategoryAnalyticsDto(
    string CategoryName,
    int TotalWords,
    int LearnedWords,
    double SuccessRatePercentage);

public record GetAnalyticsReportQuery(Guid UserId) : IRequest<BaseResponse<List<CategoryAnalyticsDto>>>;

public class GetAnalyticsReportQueryHandler : IRequestHandler<GetAnalyticsReportQuery, BaseResponse<List<CategoryAnalyticsDto>>>
{
    private readonly IWordRepository _wordRepository;
    private readonly IReviewHistoryRepository _reviewHistoryRepository;

    public GetAnalyticsReportQueryHandler(IWordRepository wordRepository, IReviewHistoryRepository reviewHistoryRepository)
    {
        _wordRepository = wordRepository;
        _reviewHistoryRepository = reviewHistoryRepository;
    }

    public async Task<BaseResponse<List<CategoryAnalyticsDto>>> Handle(GetAnalyticsReportQuery request, CancellationToken cancellationToken)
    {
        var (words, _) = await _wordRepository.GetPagedByUserIdAsync(request.UserId, 1, 10000, null, null, null, cancellationToken);
        
        var categoryGroups = words
            .Where(w => w.CategoryId.HasValue)
            .GroupBy(w => new { w.CategoryId, CategoryName = w.Category!.Name })
            .ToList();

        var reports = new List<CategoryAnalyticsDto>();

        foreach (var group in categoryGroups)
        {
            var totalWords = group.Count();
            var learnedWords = group.Count(w => w.Progresses.Any(p => p.UserId == request.UserId && p.IsLearned));

            // Optional: You can also calculate success rate based on ReviewHistories within the category
            // But doing it via Word.Progresses or counting IsLearned ratio is easier:
            var successRate = totalWords > 0 ? Math.Round((double)learnedWords / totalWords * 100, 2) : 0;

            reports.Add(new CategoryAnalyticsDto(
                group.Key.CategoryName,
                totalWords,
                learnedWords,
                successRate
            ));
        }

        return BaseResponse<List<CategoryAnalyticsDto>>.SuccessResponse(reports);
    }
}
