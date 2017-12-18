using System;
using System.Security.Cryptography;
using System.Text;

namespace ConfigStore.Workbench
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(MD5Hash("sad"));
            Console.ReadKey();
        }

        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return Encoding.ASCII.GetString(result);
            }
        }
    }
}
