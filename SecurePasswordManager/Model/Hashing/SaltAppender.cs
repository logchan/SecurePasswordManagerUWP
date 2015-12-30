using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    class SaltAppender : ISaltingServiceProvider
    {
        public string AddSalt(string input, string salt, HashingStatus status)
        {
            return input + salt;
        }
    }
}
