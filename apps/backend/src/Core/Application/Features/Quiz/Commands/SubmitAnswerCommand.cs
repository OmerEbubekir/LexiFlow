using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Quiz.Commands;

public record SubmitAnswerCommand(Guid UserId, Guid WordId, bool IsCorrect) : IRequest<BaseResponse>;

public class SubmitAnswerCommandValidator : AbstractValidator<SubmitAnswerCommand>
{
    public SubmitAnswerCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WordId).NotEmpty();
    }
}

public class SubmitAnswerCommandHandler : IRequestHandler<SubmitAnswerCommand, BaseResponse>
{
    private readonly IUserProgressRepository _userProgressRepository;
    private readonly IReviewHistoryRepository _reviewHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitAnswerCommandHandler(
        IUserProgressRepository userProgressRepository,
        IReviewHistoryRepository reviewHistoryRepository,
        IUnitOfWork unitOfWork)
    {
        _userProgressRepository = userProgressRepository;
        _reviewHistoryRepository = reviewHistoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var progress = await _userProgressRepository.GetByUserAndWordAsync(request.UserId, request.WordId, cancellationToken);

            if (progress == null)
            {
                return BaseResponse.FailureResponse("Word progress not found for this user.");
            }

            var previousStage = progress.Stage;

            // Apply 6-repetition algorithm
            progress.ApplyAnswer(request.IsCorrect);
            _userProgressRepository.Update(progress);

            // Audit log
            var history = ReviewHistory.Create(
                userId: request.UserId,
                wordId: request.WordId,
                isCorrect: request.IsCorrect,
                stageBeforeReview: previousStage);
            
            await _reviewHistoryRepository.AddAsync(history, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return BaseResponse.SuccessResponse();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
