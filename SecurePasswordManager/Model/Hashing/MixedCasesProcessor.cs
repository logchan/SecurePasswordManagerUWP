using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    /// <summary>
    /// IPostHashingProcessor. The 1st, 3rd, 5th, 7th, ... letters will become upper-cased. Input: a12bcd4e. Output: A12bCd4E.
    /// </summary>
    public class MixedCasesProcessor : IPostHashingProcessor
    {
        public string Process(string input)
        {
            StringBuilder sb = new StringBuilder();
            bool isUpper = false;
            foreach (char c in input)
            {
                if (c >= 'a' && c <= 'z')
                {
                    isUpper = !isUpper;
                    if (isUpper)
                        sb.Append((char)(c + ('A' - 'a')));
                    else
                        sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
