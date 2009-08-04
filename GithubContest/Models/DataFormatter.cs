using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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

        public static void OutputPredictions(string outputPath, TestData td, int[][] predictions)
        {
            if (File.Exists(outputPath)) File.Delete(outputPath);
            StreamWriter sw = new StreamWriter(File.OpenWrite(outputPath));
            for (int usrIndx = 0; usrIndx < predictions.Length; usrIndx++)
            {
                string outStr = td.Users[usrIndx].ExternalID + ":";
                for (int i = 0; i < predictions[usrIndx].Length; i++)
                {
                    outStr += predictions[usrIndx][i];
                    if (i < predictions[usrIndx].Length - 1) outStr += ",";
                }
                
                sw.WriteLine(outStr);
            }
            sw.Close();
        }
    }
}
