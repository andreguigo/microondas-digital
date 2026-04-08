using System.Security.Cryptography;
using System.Text;

namespace Microondas.Api.Servicos;

public static class Sha1HashService
{
    public static string Hash(string input)
    {
        var bytes = SHA1.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    public static bool EqualsHash(string input, string expectedHash)
    {
        var hash = Hash(input);
        return hash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    public static string Mask(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return new string('*', Math.Max(8, value.Length));
    }
}
