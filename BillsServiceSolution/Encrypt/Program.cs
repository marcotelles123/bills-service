using Bills.Service.Util;
using System;

namespace Encrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Type your text to encrypt: ");

            // using the method
            // typecasting not needed
            // as ReadLine returns string
            var pass = Console.ReadLine();


            Console.WriteLine("Encrypted: "+ Security.EncryptString(pass));
        }
    }
}
