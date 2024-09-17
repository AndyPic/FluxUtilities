using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Flux.Encryption
{
    public static class Encryptor
    {
        public static string AesEncrypt(string data, string iv, string key)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            csEncrypt.Write(dataBytes, 0, dataBytes.Length);
            csEncrypt.FlushFinalBlock();

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string AesDecrypt(string data, string iv, string key)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(Convert.FromBase64String(data));
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        public static string Obfuscate(string data, int offset)
        {
            StringBuilder obfuscatedData = new(data.Length);
            foreach (char c in data)
            {
                obfuscatedData.Append((char)(c + offset));
            }
            return obfuscatedData.ToString();
        }

        public static string Deobfuscate(string data, int offset)
        {
            return Obfuscate(data, -offset);
        }
    }
}
