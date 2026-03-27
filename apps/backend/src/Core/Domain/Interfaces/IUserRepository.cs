using LexiFlow.Domain.Entities;

namespace LexiFlow.Domain.Interfaces;

/// <summary>Kullanıcı deposu sözleşmesi.</summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Firebase UID ile kullanıcıyı getirir. Yoksa null döner.</summary>
    Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default);

    /// <summary>E-posta adresi ile kullanıcıyı getirir. Yoksa null döner.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Firebase UID ile kullanıcının var olup olmadığını kontrol eder.</summary>
    Task<bool> ExistsByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
}
