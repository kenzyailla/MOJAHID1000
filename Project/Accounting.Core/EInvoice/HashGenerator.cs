using System.Security.Cryptography;
using System.Text;

namespace Accounting.Core.EInvoice
{
    public static class HashGenerator
    {
        public static string GenerateHash(string input)
        {
            SHA256 sha = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(input);

            byte[] hash = sha.ComputeHash(bytes);

            return System.Convert.ToBase64String(hash);
        }
    }
}