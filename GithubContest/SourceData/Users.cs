using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class Users: List<User>
    {
        private Dictionary<int, int> Index = new Dictionary<int, int>();

        public User GetOrAddNew(int externalID)
        {
            if(Index.ContainsKey(externalID))
            {
                return this[Index[externalID]];
            }
            else
            {
                User u = new User();
                u.ID = this.Count;
                u.ExternalID = externalID;
                this.Add(u);
                Index[externalID] = u.ID;
                return u;
            }
        }
    }
}
