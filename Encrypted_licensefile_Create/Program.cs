using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encrypted_licensefile_Create
{
    class Program
    {
        //New console
        static void Main(string[] args)
        {
            string licenseText =
             @"SERIAL=L2HF82802X1 EXPIRY=2026-01-20 APP=OMS";

            string encrypted = AesEncryption.Encrypt(licenseText);

            File.WriteAllText("license.lic", encrypted);

            Console.WriteLine("License file created");
            Console.ReadLine();
        }
    }
}
