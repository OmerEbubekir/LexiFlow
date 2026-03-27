using System.Text.Json;
using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Wordle.Commands;

public record WordleGuessResultDto(string Guess, string Pattern); // Pattern like: "⬛🟨🟩⬛⬛"

public record SubmitWordleGuessResponseDto(
    bool IsCompleted,
    bool IsWon,
    List<WordleGuessResultDto> Guesses);

public record SubmitWordleGuessCommand(Guid UserId, Guid GameId, string Guess) : IRequest<BaseResponse<SubmitWordleGuessResponseDto>>;

public class SubmitWordleGuessCommandValidator : AbstractValidator<SubmitWordleGuessCommand>
{
    public SubmitWordleGuessCommandValidator()
    {
        RuleFor(x => x.GameId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Guess).NotEmpty().Length(5).WithMessage("Guess must be exactly 5 letters.");
    }
}

public class SubmitWordleGuessCommandHandler : IRequestHandler<SubmitWordleGuessCommand, BaseResponse<SubmitWordleGuessResponseDto>>
{
    private readonly IRepository<WordleGame> _wordleGameRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitWordleGuessCommandHandler(IRepository<WordleGame> wordleGameRepository, IUnitOfWork unitOfWork)
    {
        _wordleGameRepository = wordleGameRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<SubmitWordleGuessResponseDto>> Handle(SubmitWordleGuessCommand request, CancellationToken cancellationToken)
    {
        var game = await _wordleGameRepository.GetByIdAsync(request.GameId, cancellationToken);
        
        if (game == null || game.UserId != request.UserId)
            return BaseResponse<SubmitWordleGuessResponseDto>.FailureResponse("Game not found.");

        if (game.IsCompleted)
            return BaseResponse<SubmitWordleGuessResponseDto>.FailureResponse("This game is already completed.");

        var guessStr = request.Guess.ToLowerInvariant();
        var targetStr = game.TargetWord.ToLowerInvariant();

        // Retrieve existing guesses
        var currentGuesses = JsonSerializer.Deserialize<List<WordleGuessResultDto>>(game.GuessesJson) ?? new List<WordleGuessResultDto>();

        // Evaluate guess
        var pattern = EvaluateGuess(guessStr, targetStr);
        var newGuessDto = new WordleGuessResultDto(guessStr, pattern);
        currentGuesses.Add(newGuessDto);

        var isWon = guessStr == targetStr;
        var isCompleted = isWon || currentGuesses.Count >= game.MaxAttempts;

        // Update domain entity
        game.AddGuess(JsonSerializer.Serialize(currentGuesses), isWon, isCompleted);
        _wordleGameRepository.Update(game);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = new SubmitWordleGuessResponseDto(isCompleted, isWon, currentGuesses);
        return BaseResponse<SubmitWordleGuessResponseDto>.SuccessResponse(responseDto);
    }

    private string EvaluateGuess(string guess, string target)
    {
        string[] result = new string[5];
        bool[] targetUsed = new bool[5];
        bool[] guessUsed = new bool[5];

        // 1st pass: Correct exactly (Green)
        for (int i = 0; i < 5; i++)
        {
            if (guess[i] == target[i])
            {
                result[i] = "🟩";
                targetUsed[i] = true;
                guessUsed[i] = true;
            }
        }

        // 2nd pass: Exists but wrong place (Yellow) vs Not exists (Black)
        for (int i = 0; i < 5; i++)
        {
            if (guessUsed[i]) continue; // Already green

            result[i] = "⬛"; // Default black

            for (int j = 0; j < 5; j++)
            {
                if (!targetUsed[j] && target[j] == guess[i])
                {
                    result[i] = "🟨";
                    targetUsed[j] = true;
                    break;
                }
            }
        }

        return string.Join("", result);
    }
}
