using System.Security.Cryptography;
using System.Text;

namespace ConfigStore.Api.Infrastructure {
    public class Encryptor {
        public static string Ecrypt(string input) {
            using (var provider = MD5.Create()) {
                var builder = new StringBuilder();
                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(input))) {
                    builder.Append(b.ToString("x2").ToLower());
                }
                return builder.ToString();
            }
        }
    }
}