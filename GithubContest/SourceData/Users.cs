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
            /*
            User retVal = null;
            foreach (User u in this)
            {
                if (u.ID == ID)
                {
                    retVal = u;
                    break;
                }
            }
            if (retVal == null)
            {
                retVal = new User();
                retVal.ID = ID;
                this.Add(retVal);
            }
            return retVal;
             */
        }
    }
}
