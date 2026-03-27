using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Application.Features.Words.DTOs;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Enums;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Words.Commands;

public record AddWordSampleRequest(string SentenceText, string? TurkishTranslation);

public record AddWordCommand(
    Guid UserId,
    string EnglishWord,
    string TurkishTranslation,
    string? PictureUrl,
    string? AudioUrl,
    DifficultyLevel DifficultyLevel,
    Guid? CategoryId,
    List<AddWordSampleRequest> Samples) : IRequest<BaseResponse<Guid>>;

public class AddWordCommandValidator : AbstractValidator<AddWordCommand>
{
    public AddWordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EnglishWord).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TurkishTranslation).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DifficultyLevel).IsInEnum();
        RuleForEach(x => x.Samples).SetValidator(new AddWordSampleRequestValidator());
    }
}

public class AddWordSampleRequestValidator : AbstractValidator<AddWordSampleRequest>
{
    public AddWordSampleRequestValidator()
    {
        RuleFor(x => x.SentenceText).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TurkishTranslation).MaximumLength(500);
    }
}

public class AddWordCommandHandler : IRequestHandler<AddWordCommand, BaseResponse<Guid>>
{
    private readonly IWordRepository _wordRepository;
    private readonly IUserProgressRepository _userProgressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddWordCommandHandler(IWordRepository wordRepository, IUserProgressRepository userProgressRepository, IUnitOfWork unitOfWork)
    {
        _wordRepository = wordRepository;
        _userProgressRepository = userProgressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<Guid>> Handle(AddWordCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            if (await _wordRepository.ExistsForUserAsync(request.UserId, request.EnglishWord, cancellationToken))
            {
                return BaseResponse<Guid>.FailureResponse("Word already exists in your vocabulary.");
            }

            var word = Word.Create(
                request.UserId,
                request.EnglishWord,
                request.TurkishTranslation,
                request.DifficultyLevel,
                request.CategoryId,
                request.PictureUrl,
                request.AudioUrl);

            foreach (var sample in request.Samples)
            {
                word.Samples.Add(WordSample.Create(word.Id, sample.SentenceText, sample.TurkishTranslation));
            }

            await _wordRepository.AddAsync(word, cancellationToken);
            
            // Add initial 6-rep progress tracker for this user and new word
            var progress = UserWordProgress.Create(request.UserId, word.Id);
            await _userProgressRepository.AddAsync(progress, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return BaseResponse<Guid>.SuccessResponse(word.Id);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
