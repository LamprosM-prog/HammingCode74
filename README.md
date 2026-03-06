# Hamming(7,4) Error Correction in C#

This is a console application implementing the Hamming(7,4) error-correcting code from scratch.

## Features
- Encode messages into Hamming(7,4) codewords.
- Introduce single-bit errors using a random noise simulation.
- Decode and correct single-bit errors automatically.
- Reconstruct original message.

## How to Run
1. Clone the repository.
2. Open the folder in your terminal.
3. Run the program using `dotnet run`.
4. Type your message and see encoding, noise, and decoding in action.

## Example
- dotnet run
- Enter message: Hi world
- The program will encode the message, and show you the binary form of it.
- It will also show what bits were flipped due to noise and will show you the final version of the message after the error correction.

## Notes
- This project is educational and shows the underlying mechanics of Hamming code.
- This program is framework dependent, it will only run on machine with .NET 10 installed.
