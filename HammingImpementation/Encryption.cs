using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace HammingImplementation
{
   public  class Encryption
    {
        static public List<int> Encode(byte[] messageToEncode)
        {
            List<int> flatBits = new List<int>();
            List<List<int>> HammingMatrix = new List<List<int>>();
            List<List<int>> byteBitsMatrix = new List<List<int>>(); //Hamming needs the order of the bits reversed. From LSB -> MSB
            List<int> bitsReversed = new List<int>(); 
            for (int i = 0; i < messageToEncode.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bitsReversed.Add((messageToEncode[i] >> j) & 1); //This where the conversion is happening.
                }
                byteBitsMatrix.Add(new List<int>(bitsReversed));
                bitsReversed.Clear(); 
            }

            foreach (List<int> bits in byteBitsMatrix)
            {
                List<int> nibble = new();
                int bitsCounter = 0;
                foreach (int bit in bits)
                {
                    nibble.Add(bit);
                    bitsCounter++;
                    if (nibble.Count == 4) //We split the byte (8 bits) into 2 4bit nibbles.
                    {
                        HammingMatrix.Add(HammingEncodeNibble(nibble)); //Sending the nibbles to get encoded and storing them in a Matrix
                        nibble.Clear();
                        bitsCounter = 0;
                    }
                }

            }
            foreach (List<int> nibbleList in HammingMatrix)
            {
                flatBits.AddRange(nibbleList);
            }

            return flatBits;
        }


        public static List<int> HammingEncodeNibble(List<int> nibble)
        {
            int counter1 = 0;
            int counter2 = 0;
            int counter3 = 0;
            List<int> temp = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
            temp[2] = nibble[0];
            temp[4] = nibble[1];
            temp[5] = nibble[2];
            temp[6] = nibble[3]; 
            // We set our data into the hamming positions. IMPORTANT NOTE : In C# Lists are 0-indexed, but Hamming has positions of 1-7. So the index 
            // and Hamming positions differ by 1. This process could be done with a for-loop but I am hard coding the positions so it's clearer to understand.
            if (temp[2] != 0)
            {
                counter1++;
                counter2++;

            }
            if (temp[4] != 0)
            {
                counter1++;
                counter3++;
            }
            if (temp[5] != 0)
            {
                counter2++;
                counter3++;
            }
            if (temp[6] != 0)
            {
                counter1++;
                counter2++;
                counter3++;
            } //Calculating the parities based on the number of "1" in the bits
            int parity1 = counter1 % 2;
            int parity2 = counter2 % 2;
            int parity3 = counter3 % 2;
            temp[0] = parity1;
            temp[1] = parity2;
            temp[3] = parity3; // Set the parities into Hamming positions 1(2^0) , 2(2^1) , 4(2^2)

            return temp;
        }

        public static List<int> HammingDecodeNibbles(List<int> encodedNibbles)
        {
            List<int> HammingDecodedNibbles = new List<int> { 0, 0, 0, 0 };
            int s1 = encodedNibbles[0] ^ encodedNibbles[2] ^ encodedNibbles[4] ^ encodedNibbles[6];
            int s2 = encodedNibbles[1] ^ encodedNibbles[2] ^ encodedNibbles[5] ^ encodedNibbles[6];
            int s3 = encodedNibbles[3] ^ encodedNibbles[4] ^ encodedNibbles[5] ^ encodedNibbles[6];
            int syndrome = s1 + (s2 << 1) + (s3 << 2);
            if (syndrome != 0) // This is where the error-correcting happens. The syndrome will be a 3 digit binary number that will tell us which (hamming) position is flipped.
            {
                encodedNibbles[syndrome - 1] ^= 1;
                Console.WriteLine($"Detetected error at position {syndrome - 1} and corrected it");

            }
            HammingDecodedNibbles[0] = encodedNibbles[2];
            HammingDecodedNibbles[1] = encodedNibbles[4];
            HammingDecodedNibbles[2] = encodedNibbles[5];
            HammingDecodedNibbles[3] = encodedNibbles[6]; //Taking only the data and not the parities for the byte restoration.

            return HammingDecodedNibbles;
        }

        public static string Decode(List<int> messageToDecode)
        {
            List<int> encodedNimbles = new List<int>();
            List<List<int>> decodedMatrixNimbles = new List<List<int>>();
            List<int> decodedBitsInts = new List<int>();
            List<byte> decodedByte = new List<byte>();
            byte byteValue = 0;
            int counter = 0;

            foreach (int bit in messageToDecode)
            {
                encodedNimbles.Add(bit);
                counter++;
                if (counter == 7)
                {
                    decodedMatrixNimbles.Add(HammingDecodeNibbles(Noise(encodedNimbles)));  //Sending each encoded Nibble (4 data, 3 parity) to a noise channel, and then cleaning it.
                    encodedNimbles.Clear();
                    counter = 0;
                }
            }
            foreach (List<int> decodedNimbles in decodedMatrixNimbles)
            {
                decodedBitsInts.AddRange(decodedNimbles); //Saving all data bits to an int list
            }
            int bytesCount = decodedBitsInts.Count / 8;
            for (int i = 0; i < bytesCount; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    byteValue |= (byte)(decodedBitsInts[i * 8 + j] << (j)); //Restoring the byte. i * 8 + j is the bit number with j being its position in the byte.

                }

                decodedByte.Add(byteValue);
                byteValue = 0;
            }

            string messageDecoded = Encoding.ASCII.GetString(decodedByte.ToArray()); //Translating from Binary -> ASCII -> Text-string.
            return messageDecoded;
        }

        public static List<int> Noise(List<int> encodedNibble)
        {
            Random rng = new Random();

            for (int i = 0; i < encodedNibble.Count; i += 7)
            {
                int randomBitIndex = rng.Next(0, 7);
                int absoluteIndex = i + randomBitIndex;
                encodedNibble[absoluteIndex] ^= 1;
                Console.WriteLine($"Flipped bit at block starting {i}, position {randomBitIndex}");
            } //The noise simulator. Just an RNG that will flip a random bit.

            return encodedNibble;
        }





    }
}
