using Microsoft.AspNetCore.Identity;

namespace API.ML.Utility
{
    public static class PasswordHasherService
    {
        public static string HashPassword(string password)
        {
            return (new PasswordHasher<string>()).HashPassword("", password.Trim());
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var result = (new PasswordHasher<string>()).VerifyHashedPassword("", hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
