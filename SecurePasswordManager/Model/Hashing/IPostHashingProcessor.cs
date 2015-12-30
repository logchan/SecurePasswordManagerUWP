using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    public interface IPostHashingProcessor
    {
        string Process(string input);
    }
}
