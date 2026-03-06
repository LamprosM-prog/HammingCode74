using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HammingImplementation
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Welcome! Write a message and the program will encode and decode it using Hamming(7,4).\nIt will also simulate noise that will flip random bits.");
            Console.Write("You:");
            byte[] messageToEncode = Encoding.ASCII.GetBytes(Console.ReadLine());
            var temp = Encryption.Encode(messageToEncode);
            string messageEncoded = string.Join("", temp);
           string messageDecoded = Encryption.Decode(temp);
            Console.WriteLine($"Encoded message before noise : {messageEncoded}");
            Console.WriteLine($"Decoded message : {messageDecoded}");
            Console.WriteLine("If you want the program to close press 'Y'. Otherwise hit 'N' to run another test.");
            string input = Console.ReadLine();
            if (input == "Y")
                return;
            else if (input == "N")
                Main(args);

        }
    }
    
}