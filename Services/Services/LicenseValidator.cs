using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Services.Services
{
    public static class LicenseValidator
    {
        public static bool IsValid(out string message)
        {
            message = "";

            try
            {

                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Dhruva",
                    "OMS",
                    "license.lic");

                if (!File.Exists(path))
                {
                    message = "License file not found";
                    return false;
                }

                string encrypted = File.ReadAllText(path);
                string decrypted = AesEncryption.Decrypt(encrypted);

                string serial = "";
                DateTime expiry = DateTime.MinValue;
                string app = "";

                var parts = decrypted.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    if (part.StartsWith("SERIAL="))
                        serial = part.Substring("SERIAL=".Length).Trim();

                    else if (part.StartsWith("EXPIRY="))
                        expiry = DateTime.Parse(
                            part.Substring("EXPIRY=".Length).Trim());

                    else if (part.StartsWith("APP="))
                        app = part.Substring("APP=".Length).Trim();
                }

                string machineSerial = GetMotherboardSerial();

                if (DateTime.Now > expiry)
                {
                    message = "License expired";
                    return false;
                }

                if (serial != machineSerial)
                {
                    message = "License not valid for this machine";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                message = "License validation failed";
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static string GetMotherboardSerial()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c wmic baseboard get serialnumber",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Clean output
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return lines.Length > 1 ? lines[1].Trim() : string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting motherboard serial:");
                Console.WriteLine(ex.ToString());

                return string.Empty;
            }

        }
       
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

        public static class LicenseState
        {
            public static bool IsValid { get; set; }
            public static string Message { get; set; }
        }

    }

}
