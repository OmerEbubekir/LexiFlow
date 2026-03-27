using FluentValidation;
using LexiFlow.Application.Common;
using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Auth.Commands;

public record ProvisionUserCommand(string FirebaseUid, string Email, string DisplayName) : IRequest<BaseResponse<Guid>>;

public class ProvisionUserCommandValidator : AbstractValidator<ProvisionUserCommand>
{
    public ProvisionUserCommandValidator()
    {
        RuleFor(x => x.FirebaseUid).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ProvisionUserCommandHandler : IRequestHandler<ProvisionUserCommand, BaseResponse<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProvisionUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<Guid>> Handle(ProvisionUserCommand request, CancellationToken cancellationToken)
    {
        // JIT Provisioning
        var user = await _userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);

        if (user == null)
        {
            user = User.Create(request.FirebaseUid, request.Email, request.DisplayName);
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return BaseResponse<Guid>.SuccessResponse(user.Id);
    }
}
