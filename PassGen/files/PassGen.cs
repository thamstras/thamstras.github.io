using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace PassGen
{
	class PassGen
	{
		private const byte COUNT = 1;
		private const byte PASS_LENGTH = 12;
		private const byte MODE = 3; // 1 = standard, 2 = additional, 3 = standard and additional, 4 = extended, 5 = standard and extended, 6 = additional and extended, 7 = standard, additional and extended.
		private const string STANDARD_CHARS = "abcdefgijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string ADDITIONAL_CHARS = "1234567890";
		//private const string EXTENDED_CHARS = "!£$%^&*_+-=:@~;#<>?,./\\";
		//reduced extended char set to increase compatability.
		private const string EXTENDED_CHARS = "!£$%&*+-=@~#?";
		private const string HELP_STRING = "PassGen: Random password generator\nArguments:\n-l\tLength\tLength of passwords to be generated.\n-m\tMode\tSelects which character set to use.\n-c\tCount\tHow many passwords to generate.\n";

		private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

		// rng returns [0, 255]. Char returned is determined by rng % charLength.
		// Therefore the charset repeats over the range of the rng with some left over
		// at the top. This function checks if the number generated falls within the bit
		// at the top and returns false if it is, so it can be rejected, preventing bias in
		// the character generation.
		private static bool IsFairRoll(byte roll, byte num)
		{
			int fullSetsOfValues = Byte.MaxValue / num;
			return roll < num * fullSetsOfValues;
		}

		// Returns a single char from the input string.
		// Choice of character is random and fair, ie:
		// all characters have an equal probablility of
		// being returned.
		private static char rollChar(string chars)
		{
			byte length = (byte)chars.Length;
			if (length <= 0)
				throw new ArgumentOutOfRangeException("charsLength");

			byte[] randomNumber = new byte[1];
			do
			{
				rngCsp.GetBytes(randomNumber);
			}
			while (!IsFairRoll(randomNumber[0], length));

			int charI = (randomNumber[0] % length);
			return chars[charI];
		}
		
		// Returns the concatinated string to use as the char set
		// based on the mode set.
		private static string doCharSelection(int mode)
		{
			switch (mode)
			{
				case 1:
					return STANDARD_CHARS;
				case 2:
					return ADDITIONAL_CHARS;
				case 3:
					return STANDARD_CHARS + ADDITIONAL_CHARS;
				case 4:
					return EXTENDED_CHARS;
				case 5:
					return STANDARD_CHARS + EXTENDED_CHARS;
				case 6:
					return ADDITIONAL_CHARS + EXTENDED_CHARS;
				case 7:
					return STANDARD_CHARS + ADDITIONAL_CHARS + EXTENDED_CHARS;
				default:
					throw new ArgumentOutOfRangeException("charsMode");
			}
		}

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Write(HELP_STRING);
				return;
			}
			int mode = MODE;
			int passLength = PASS_LENGTH;
			int count = COUNT;
			// Command line arguments processing.
			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				if (arg == "-m" || arg == "/m")
				{
					mode = Int32.Parse(args[i+1]);
					continue;
				}
				if (arg == "-l" || arg == "/l")
				{
					passLength = Int32.Parse(args[i+1]);
					continue;
				}
				if (arg == "-c" || arg == "/c")
				{
					count = Int32.Parse(args[i+1]);
					continue;
				}
			}
			
			Console.WriteLine("---PassGen: Length = {0}, Mode = {1}, Count = {2}---", passLength, mode, count);
			
			string chars = doCharSelection(mode);
			char[] pass = new char[passLength];
			for (int c = 0; c < count; c++)
			{
				for (int i = 0; i < passLength; i++)
				{
					pass[i] = rollChar(chars);
				}
				Console.WriteLine(pass);
			}
			rngCsp.Dispose();
		}
	}
}
