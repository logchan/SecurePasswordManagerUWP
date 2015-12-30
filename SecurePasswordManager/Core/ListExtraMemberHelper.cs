using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Core
{
    /// <summary>
    /// This class is for adding some static members before / after a list when it is used
    /// as an IEnumerable, e.g. binding to a ListView, so that these members do not need
    /// to be added to that list when it is updated. Please use it carefully.
    /// </summary>
    public class ListExtraMemberHelper<T> : IEnumerable
    {
        public List<T> membersBefore;
        public List<T> members;
        public List<T> membersAfter;

        public ListExtraMemberHelper()
        {
            membersBefore = null;
            members = null;
            membersAfter = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ListExtraMemberHelperEnum<T>(this);
        }
    }

    public class ListExtraMemberHelperEnum<T> : IEnumerator
    {
        ListExtraMemberHelper<T> helper;
        int index = -1;

        public ListExtraMemberHelperEnum(ListExtraMemberHelper<T> h)
        {
            helper = h;
        }

        object IEnumerator.Current
        {
            get
            {
                int i = index;
                if (helper.membersBefore != null)
                {
                    if (i < helper.membersBefore.Count)
                        return helper.membersBefore[i];
                    else
                        i -= helper.membersBefore.Count;
                }
                if (helper.members != null)
                {
                    if (i < helper.members.Count)
                        return helper.members[i];
                    else
                        i -= helper.members.Count;
                }
                if (helper.membersAfter != null)
                {
                    if (i < helper.membersAfter.Count)
                        return helper.membersAfter[i];
                    else
                        i -= helper.membersAfter.Count;
                }
                return default(T);
            }
        }

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            ++index;
            int total = 0;
            if (helper.membersBefore != null)
                total += helper.membersBefore.Count;
            if (helper.members != null)
                total += helper.members.Count;
            if (helper.membersAfter != null)
                total += helper.membersAfter.Count;

            return (index < total);
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
