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

        public void Run(TrainingData td, TestData test, string outPath)
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
                Repository[] repos = userA.Repo.GetList();
                int[] userMatches = new int[td.Users.Count];
                for (int rIndx = 0; rIndx < repos.Length; rIndx++)
                {
                    Repository r = repos[rIndx];
                    List<int> users = mod[r.ID];
                    foreach (int userB in users) 
                    {
                        if(userB == userA.ID) continue;
                        userMatches[userB]++;
                    }
                }
                // now weighted repo matches
                int[] repoMatches = new int[td.Repositories.Count];
                for (int userB = 0; userB < uod.Length; userB++)
                {
                    if (userB == userA.ID) continue;
                    if (userMatches[userB] > 0)
                    {
                        int weight = userMatches[userB] * userMatches[userB];

                        foreach (int repo in uod[userB])
                        {
                            repoMatches[repo] += weight;
                        }
                    }
                }
                // find x highest
                predictions[usrIndx] = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    int highRepo = 0;
                    int highWeight = 0;

                    for (int j = 0; j < repoMatches.Length; j++)
                    {
                        int weight = repoMatches[j];
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
            DataFormatter.OutputPredictions(outPath, test, predictions);
        }
    }
}
