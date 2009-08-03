using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class User
    {
        public int ID;
        public int ExternalID;
        public Repositories Repo = new Repositories(); 
    }
}
