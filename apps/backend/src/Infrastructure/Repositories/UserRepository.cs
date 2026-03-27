using LexiFlow.Domain.Entities;
using LexiFlow.Domain.Interfaces;
using LexiFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LexiFlow.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default)
    {
        return await DbContext.Users.AnyAsync(u => u.FirebaseUid == firebaseUid, cancellationToken);
    }
}
