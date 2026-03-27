using LexiFlow.Application.Common;
using LexiFlow.Domain.Interfaces;
using MediatR;

namespace LexiFlow.Application.Features.Auth.Queries;

public record UserSettingsDto(int DailyNewWordLimit, string DisplayName);

public record GetUserSettingsQuery(Guid UserId) : IRequest<BaseResponse<UserSettingsDto>>;

public class GetUserSettingsQueryHandler : IRequestHandler<GetUserSettingsQuery, BaseResponse<UserSettingsDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserSettingsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<UserSettingsDto>> Handle(GetUserSettingsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<UserSettingsDto>.FailureResponse("User not found.");

        return BaseResponse<UserSettingsDto>.SuccessResponse(new UserSettingsDto(user.DailyNewWordLimit, user.DisplayName));
    }
}
