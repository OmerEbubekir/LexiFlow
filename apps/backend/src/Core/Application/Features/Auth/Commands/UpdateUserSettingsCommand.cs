using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Auth.Commands;

public record UpdateUserSettingsCommand(Guid UserId, int DailyNewWordLimit) : IRequest<BaseResponse>;

public class UpdateUserSettingsCommandValidator : AbstractValidator<UpdateUserSettingsCommand>
{
    public UpdateUserSettingsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DailyNewWordLimit).InclusiveBetween(1, 100);
    }
}

public class UpdateUserSettingsCommandHandler : IRequestHandler<UpdateUserSettingsCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserSettingsCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResponse.FailureResponse("User not found.");

        user.UpdateDailyNewWordLimit(request.DailyNewWordLimit);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse.SuccessResponse();
    }
}
