using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Enums;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Words.Commands;

public record UpdateWordCommand(
    Guid WordId,
    Guid UserId,
    string EnglishWord,
    string TurkishTranslation,
    string? PictureUrl,
    string? AudioUrl,
    DifficultyLevel DifficultyLevel,
    Guid? CategoryId,
    List<AddWordSampleRequest> Samples) : IRequest<BaseResponse>;

public class UpdateWordCommandValidator : AbstractValidator<UpdateWordCommand>
{
    public UpdateWordCommandValidator()
    {
        RuleFor(x => x.WordId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EnglishWord).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TurkishTranslation).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DifficultyLevel).IsInEnum();
        RuleForEach(x => x.Samples).SetValidator(new AddWordSampleRequestValidator()); // Reusing from AddWordCommand
    }
}

public class UpdateWordCommandHandler : IRequestHandler<UpdateWordCommand, BaseResponse>
{
    private readonly IWordRepository _wordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWordCommandHandler(IWordRepository wordRepository, IUnitOfWork unitOfWork)
    {
        _wordRepository = wordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> Handle(UpdateWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _wordRepository.GetWordWithSamplesAsync(request.WordId, cancellationToken);
        
        if (word == null || word.UserId != request.UserId)
            return BaseResponse.FailureResponse("Word not found.");

        word.Update(
            request.EnglishWord,
            request.TurkishTranslation,
            request.DifficultyLevel,
            request.CategoryId,
            request.PictureUrl,
            request.AudioUrl);

        if (word.Samples != null && word.Samples.Any())
        {
            await _wordRepository.DeleteSamplesAsync(word.Samples.ToList());
            word.Samples.Clear();
        }

        var newSamples = new List<WordSample>();
        foreach (var sample in request.Samples)
        {
            var newSample = WordSample.Create(word.Id, sample.SentenceText, sample.TurkishTranslation);
            word.Samples!.Add(newSample);
            newSamples.Add(newSample);
        }

        if (newSamples.Any())
        {
            await _wordRepository.AddSamplesAsync(newSamples);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse.SuccessResponse();
    }
}
