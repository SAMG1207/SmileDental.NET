using Microsoft.AspNetCore.Identity;

namespace SmileDental.Utils
{
    public class PasswordManager
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordManager(PasswordHasher<object> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
