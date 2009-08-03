using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class DataFormatter
    {
        public static int[][] GetUserOrderRepositories(TrainingData td)
        {
            int usrCount = td.Users.Count;
            int[][] retVal = new int[usrCount][];
            foreach (User u in td.Users)
            {
                Repository[] reps = u.Repo.GetList(); 
                retVal[u.ID] = new int[reps.Length];
                for (int i = 0; i < reps.Length; i++)
                    retVal[u.ID][i] = reps[i].ID;
            }
            return retVal;
        }
    }
}
