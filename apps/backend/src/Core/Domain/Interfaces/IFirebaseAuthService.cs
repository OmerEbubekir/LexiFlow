namespace LexiFlow.Domain.Interfaces;

public interface IFirebaseAuthService
{
    Task<string> GeneratePasswordResetLinkAsync(string email);
}
