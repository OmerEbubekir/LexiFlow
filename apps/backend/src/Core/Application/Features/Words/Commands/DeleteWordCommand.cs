using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Words.Commands;

public record DeleteWordCommand(Guid WordId, Guid UserId) : IRequest<BaseResponse>;

public class DeleteWordCommandValidator : AbstractValidator<DeleteWordCommand>
{
    public DeleteWordCommandValidator()
    {
        RuleFor(x => x.WordId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public class DeleteWordCommandHandler : IRequestHandler<DeleteWordCommand, BaseResponse>
{
    private readonly IWordRepository _wordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWordCommandHandler(IWordRepository wordRepository, IUnitOfWork unitOfWork)
    {
        _wordRepository = wordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> Handle(DeleteWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);
        
        if (word == null || word.UserId != request.UserId)
            return BaseResponse.FailureResponse("Word not found.");

        _wordRepository.Delete(word);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse.SuccessResponse();
    }
}
