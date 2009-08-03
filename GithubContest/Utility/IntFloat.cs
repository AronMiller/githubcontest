using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GithubContest
{
    public class IntFloat: IComparable 
    {
        public int Int;
        public float Float;

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            IntFloat comp = obj as IntFloat;
            if (comp.Float < this.Float)
                return -1;
            else if (comp.Float > this.Float)
                return 1;
            else
                return 0;
        }

        #endregion
    }
}
