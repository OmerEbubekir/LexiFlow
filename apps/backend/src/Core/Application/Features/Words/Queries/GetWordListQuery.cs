using LexiFlow.Application.Common;
using LexiFlow.Application.Features.Words.DTOs;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Words.Queries;

public record GetWordListQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? CategoryId = null,
    bool? IsLearned = null) : IRequest<BaseResponse<PaginatedList<WordListItemDto>>>;

public class GetWordListQueryHandler : IRequestHandler<GetWordListQuery, BaseResponse<PaginatedList<WordListItemDto>>>
{
    private readonly IWordRepository _wordRepository;

    public GetWordListQueryHandler(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }

    public async Task<BaseResponse<PaginatedList<WordListItemDto>>> Handle(GetWordListQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _wordRepository.GetPagedByUserIdAsync(
            request.UserId,
            request.Page,
            request.PageSize,
            request.Search,
            request.CategoryId,
            request.IsLearned,
            cancellationToken);

        var dtoList = items.Select(w => new WordListItemDto(
            w.Id,
            w.EnglishWord,
            w.TurkishTranslation,
            w.DifficultyLevel,
            w.Category?.Name)).ToList();

        var paginatedList = new PaginatedList<WordListItemDto>(dtoList, totalCount, request.Page, request.PageSize);

        return BaseResponse<PaginatedList<WordListItemDto>>.SuccessResponse(paginatedList);
    }
}
