using System;
using System.Security.Cryptography;
using System.Text;

namespace ConfigStore.Api.Infrastructure {
    public class Encryptor {
        public static string EcryptOneWay(string input) {
            using (var provider = MD5.Create()) {
                var builder = new StringBuilder();
                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(input))) {
                    builder.Append(b.ToString("x2").ToLower());
                }
                return builder.ToString();
            }
        }

        public static string Ecrypt(string input) {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            var builder = new StringBuilder();
            foreach (byte b in bytes) {
                builder.Append(b.ToString("x2").ToLower());
            }
            return builder.ToString();
        }

        public static string Decrypt(string input) {
            int length = input.Length / 2;
            var bytes = new byte[length];
            for (int i = 0; i < length; i++) {
                bytes[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
            }
            return Encoding.UTF8.GetString(bytes);
        }
    }
}