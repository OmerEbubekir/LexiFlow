using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;

using LexiFlow.Domain.Interfaces;

namespace LexiFlow.Infrastructure.ExternalServices;

public class FirebaseService : IFirebaseAuthService
{
    private readonly IConfiguration _config;
    // FirebaseAuth instance could be resolved if properly registered.
    // For simplicity, we assume FirebaseAdmin is initialized globally in Program.cs
    
    public FirebaseService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
    {
        // This relies on FirebaseApp.Create() being called earlier with valid credentials.
        return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
    }

    public async Task<string> GeneratePasswordResetLinkAsync(string email)
    {
        // Note: The built-in Firebase FirebaseUser.SendPasswordResetEmail
        // is client-side. Server-side can generate a password reset link to be emailed.
        // For standard Firebase approach in a backend, we can generate a link:
        return await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email);
    }
}
