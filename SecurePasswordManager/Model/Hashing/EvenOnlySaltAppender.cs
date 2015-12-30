using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    public class EvenOnlySaltAppender : ISaltingServiceProvider
    {
        public string AddSalt(string input, string salt, HashingStatus status)
        {
            if (status.CurrentIteration % 2 == 0)
                return input + salt;
            else
                return input;
        }
    }
}
