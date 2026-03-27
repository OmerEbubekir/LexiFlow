using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<BaseResponse>;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFirebaseAuthService _firebaseService;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IFirebaseAuthService firebaseService)
    {
        _userRepository = userRepository;
        _firebaseService = firebaseService;
    }

    public async Task<BaseResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            // Do not leak user existence. Pretend it worked.
            return BaseResponse.SuccessResponse();
        }

        try
        {
            await _firebaseService.GeneratePasswordResetLinkAsync(request.Email);
            // Typically, you would send this link via email here using an EmailService,
            // or if using client-side Firebase Auth, the frontend handles sendPasswordResetEmail.
            // But we simulate handling it.
            return BaseResponse.SuccessResponse();
        }
        catch (Exception ex)
        {
            return BaseResponse.FailureResponse($"Failed to generate reset link: {ex.Message}");
        }
    }
}
