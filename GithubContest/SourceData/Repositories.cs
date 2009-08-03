using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class Repositories: List<Repository>
    {
        private Dictionary<int, int> internalIndex = new Dictionary<int, int>();
        private Dictionary<int, int> externalIndex = new Dictionary<int, int>();

        internal Repository GetByExternID(int externID)
        {
            if (externalIndex.ContainsKey(externID))
                return this[externalIndex[externID]];
            else
                return null;
        }
        
        internal Repository GetByInternalID(int internalID)
        {
            if (internalIndex.ContainsKey(internalID))
                return this[internalIndex[internalID]];
            else
                return null;
        }

        public void CreateIndex()
        {
            for(int i = 0; i < this.Count; i++) 
            {
                Repository r = this[i];
                externalIndex[r.ExternalID] = i;
                internalIndex[r.ID] = i;
            }
        }
    }
}
