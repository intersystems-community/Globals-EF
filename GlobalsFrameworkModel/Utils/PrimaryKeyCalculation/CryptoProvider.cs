using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GlobalsFramework.Utils.PrimaryKeyCalculation
{
    internal static class CryptoProvider
    {
        internal static string CalculatePrimaryKeyHash(NameValueCollection primaryKeys)
        {
            var keyValues = from key in primaryKeys.AllKeys
                            from value in primaryKeys.GetValues(key)
                            select string.Format("{0}={1}", key, value);

            using (var sha256 = SHA256.Create())
            {
                var buffer = Encoding.UTF8.GetBytes(string.Join("&", keyValues));
                var hash = sha256.ComputeHash(buffer, 0, buffer.Length);

                var sBuilder = new StringBuilder();
 
                foreach (var c in hash)
                {
                    sBuilder.Append(c.ToString("x"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
