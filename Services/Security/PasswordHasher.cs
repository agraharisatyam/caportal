using System.Security.Cryptography;

namespace caportal.Services.Security;

/// <summary>
/// Provides PBKDF2 HMAC-SHA256 salted password hashing and verification.
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;       // 128 bit
    private const int HashSize = 32;       // 256 bit
    private const int Iterations = 10000;

    /// <summary>
    /// Hashes a plain text password with a random salt.
    /// Returns salt:hash in Base64 format.
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verifies a password against a stored salt:hash string.
    /// </summary>
    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            return false;

        var parts = storedHash.Split(':');
        if (parts.Length != 2)
            return false;

        try
        {
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
        catch
        {
            return false;
        }
    }
}
