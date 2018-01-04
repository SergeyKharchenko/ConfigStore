using System;
using System.Security.Cryptography;
using System.Text;

namespace ConfigStore.Workbench
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(MD5("a"));
            Console.ReadKey();
        }

        public static string MD5(string s)
        {
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                StringBuilder builder = new StringBuilder();                           

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
                    builder.Append(b.ToString("x2").ToLower());

                return builder.ToString();
            }
        }
    }
}
