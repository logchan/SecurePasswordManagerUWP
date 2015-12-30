using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    public interface IHashingServiceProvider
    {
        string Hash(string input);
    }
}
