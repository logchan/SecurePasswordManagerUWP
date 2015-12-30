using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Scheme
{
    public enum SPMSchemeSaltingType
    {
        ALWAYS,
        ADD_ONCE,
        ODD_ONLY,
        EVEN_ONLY
    }
}
