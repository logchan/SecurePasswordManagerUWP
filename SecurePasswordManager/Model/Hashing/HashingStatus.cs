using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    public class HashingStatus
    {
        private int currentIteration;

        public int CurrentIteration
        {
            get { return currentIteration; }
            set { currentIteration = value; }
        }

    }
}
