using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    public class HexToCharsProcessor : IPostHashingProcessor
    {
        private string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string Process(string input)
        {
            if (input.Length % 2 != 0)
                return input;

            StringBuilder result = new StringBuilder();
            int lenCharArr = characters.Length;

            input = input.ToLower();
            for (int i = 0; i < input.Length - 1; i += 2)
            {
                int digit1 = input[i] >= 'a' ? input[i] - 'a' : input[i] - '0';
                int digit2 = input[i + 1] >= 'a' ? input[i + 1] - 'a' : input[i + 1] - '0';
                int num = digit1 * 16 + digit2;
                result.Append(characters[num % lenCharArr]);
            }

            return result.ToString();
        }
    }
}
