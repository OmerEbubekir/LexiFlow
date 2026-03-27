using LexiFlow.Application.Common;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using MediatR;
using System.Security.Cryptography;

namespace LexiFlow.Application.Features.Wordle.Commands;

public record StartWordleGameResponseDto(Guid GameId, int MaxAttempts);

public record StartWordleGameCommand(Guid UserId) : IRequest<BaseResponse<StartWordleGameResponseDto>>;

public class StartWordleGameCommandHandler : IRequestHandler<StartWordleGameCommand, BaseResponse<StartWordleGameResponseDto>>
{
    private readonly IWordRepository _wordRepository;
    private readonly IRepository<WordleGame> _wordleGameRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartWordleGameCommandHandler(
        IWordRepository wordRepository,
        IRepository<WordleGame> wordleGameRepository,
        IUnitOfWork unitOfWork)
    {
        _wordRepository = wordRepository;
        _wordleGameRepository = wordleGameRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<StartWordleGameResponseDto>> Handle(StartWordleGameCommand request, CancellationToken cancellationToken)
    {
        var fiveLetterWords = await _wordRepository.GetLearnedFiveLetterWordsByUserIdAsync(request.UserId, cancellationToken);
        
        if (!fiveLetterWords.Any())
            return BaseResponse<StartWordleGameResponseDto>.FailureResponse("You don't have enough learned 5-letter words to play Wordle.");

        // Pick random target word
        var randomIndex = RandomNumberGenerator.GetInt32(0, fiveLetterWords.Count);
        var targetWord = fiveLetterWords[randomIndex].EnglishWord;

        var game = WordleGame.Create(request.UserId, targetWord);
        
        await _wordleGameRepository.AddAsync(game, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse<StartWordleGameResponseDto>.SuccessResponse(new StartWordleGameResponseDto(game.Id, game.MaxAttempts));
    }
}
