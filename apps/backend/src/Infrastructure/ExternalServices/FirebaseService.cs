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
        if (FirebaseAuth.DefaultInstance == null)
        {
            // Simulate link generation as Firebase Admin SDK is not initialized with Service Account Key
            await Task.Delay(100);
            return $"https://lexiflow.com/reset-password?email={email}&token=mock-token";
        }
        
        return await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email);
    }
}
