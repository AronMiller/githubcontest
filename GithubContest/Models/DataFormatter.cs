using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class DataFormatter
    {
        public static int[][] GetUserOrderRepositories(TrainingData td)
        {
            int usrCount= td.Users.Count;
            int repoCount = td.Repositories.Count;
            int[][] retVal = new int[usrCount][];
            foreach (User u in td.Users)
            {
                retVal[u.ID] = new int[u.Repo.Count];
                for (int i = 0; i < u.Repo.Count; i++)
                    retVal[u.ID][i] = u.Repo[i].ID;
            }
            return retVal;
        }
    }
}
