using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class Repository
    {
        public int ID;
        public int ExternalID;
        public DateTime DateCreated;
        public string Name;
        public List<RepoLanguage> Languages= new List<RepoLanguage>();
    }
}
