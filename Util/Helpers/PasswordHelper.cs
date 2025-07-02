namespace PubQuizBackend.Util.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            passwordSalt = new byte[SaltSize];
            rng.GetBytes(passwordSalt);

            using var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, passwordSalt, Iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
            passwordHash = pbkdf2.GetBytes(KeySize);
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, storedSalt, Iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(KeySize);
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
