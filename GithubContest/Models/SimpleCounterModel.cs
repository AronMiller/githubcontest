using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GithubContest
{
    public class SimpleCounterModel
    {
        // for each user in test
        // find similar users, count up 

        public int[][] Run(TrainingData td, TestData test, string outPath)
        {
            int[][] uod = DataFormatter.GetUserOrderRepositories(td);

            List<int>[] mod = new List<int>[td.Repositories.Count];

            for (int i = 0; i < td.Repositories.Count; i++)
                mod[i] = new List<int>();

            // populate mod
            for (int i = 0; i < uod.Length; i++)
            {
                for (int j = 0; j < uod[i].Length; j++)
                {
                    mod[uod[i][j]].Add(i);
                }
            }

            int[][] predictions = new int[test.Users.Count][];

            for (int usrIndx = 0; usrIndx < test.Users.Count; usrIndx++)
            {
                User userA = test.Users[usrIndx];

                // Figure out user-user weights
                int[] repos = uod[userA.ID];
                int[] userMatches = new int[td.Users.Count];
                for (int rIndx = 0; rIndx < repos.Length; rIndx++)
                {
                    int rID = repos[rIndx];
                    List<int> users = mod[rID];
                    foreach (int userB in users) 
                    {
                        if(userB == userA.ID) continue;
                        userMatches[userB]++;
                    }
                }
                float[] userWeights = new float[td.Users.Count];
                for (int i = 0; i < userMatches.Length; i++)
                {
                    userWeights[i] = (float)(userMatches[i] / (Math.Sqrt(uod[userA.ID].Length) * Math.Sqrt(uod[i].Length)));
                }

                // now weighted repo matches
                float[] repoMatches = new float[td.Repositories.Count];
                for (int userB = 0; userB < uod.Length; userB++)
                {
                    if (userB == userA.ID) continue;
                    if (userMatches[userB] > 0)
                    {
                        float weight = userWeights[userB] * userWeights[userB];

                        foreach (int repo in uod[userB])
                        {
                            repoMatches[repo] += weight;
                        }
                    }
                }

                // strike out all repos already watched
                foreach (int rID in uod[userA.ID])
                {
                    repoMatches[rID] = 0;
                }


                // find x highest
                predictions[usrIndx] = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    int highRepo = 0;
                    float highWeight = 0;

                    for (int j = 0; j < repoMatches.Length; j++)
                    {
                        float weight = repoMatches[j];
                        if (weight > highWeight)
                        {
                            highRepo = j;
                            highWeight = weight;
                        }
                    }
                    repoMatches[highRepo] = 0; // zero out to skip next round
                    predictions[usrIndx][i] = highRepo;
                }
            }
            return predictions;
            //DataFormatter.OutputPredictions(outPath, td, test, predictions);
        }
    }
}
