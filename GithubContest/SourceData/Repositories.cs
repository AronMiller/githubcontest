using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class Repositories
    {
        private List<Repository> allReps = new List<Repository>();
        private Dictionary<int, Repository> internalIndex = new Dictionary<int, Repository>();
        private Dictionary<int, Repository> externalIndex = new Dictionary<int, Repository>();

        internal Repository GetByExternID(int externID)
        {
            if (externalIndex.ContainsKey(externID))
                return externalIndex[externID];
            else
                return null;
        }
        
        internal Repository GetByInternalID(int internalID)
        {
            if (internalIndex.ContainsKey(internalID))
                return internalIndex[internalID];
            else
                return null;
        }

        public void AddRepository(Repository r)
        {
            allReps.Add(r);
            externalIndex[r.ExternalID] = r;
            internalIndex[r.ID] = r;
        }
        public int Count
        {
            get { return allReps.Count; }
        }

        public Repository[] GetList()
        {
            return allReps.ToArray();
        }
    }
}
