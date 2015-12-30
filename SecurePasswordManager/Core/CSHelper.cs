using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Core
{
    public static class CSHelper
    {
        public static int IndexOfEnum<TEnum>(IList<object> list, TEnum value) where TEnum : IComparable
        {
            int result = -1;
            for (int i = 0; i < list.Count; ++i)
                if (value.CompareTo(list[i]) == 0)
                    result = i;

            return result;
        }
    }
}
