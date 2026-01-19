using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encrypted_licensefile_Create
{
    public class AesEncryption
    {
        //  KEEP SECRET (do NOT commit to Git)
        private static readonly byte[] Key =
            //Encoding.UTF8.GetBytes("1234567890ABCDEF"); // 16 bytes
            Encoding.UTF8.GetBytes("45643536dgfsc3we");
        private static readonly byte[] IV =
            //Encoding.UTF8.GetBytes("ABCDEF1234567890"); // 16 bytes
            Encoding.UTF8.GetBytes("sgrcxesgeg43563i");
        public static string Encrypt(string plainText)
        {
             var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

             var encryptor = aes.CreateEncryptor();
             var ms = new MemoryStream();
             var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
             var sw = new StreamWriter(cs);

            sw.Write(plainText);
            sw.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
             var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

             var decryptor = aes.CreateDecryptor();
             var ms = new MemoryStream(Convert.FromBase64String(cipherText));
             var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
             var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
    }
}
