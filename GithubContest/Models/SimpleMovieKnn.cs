using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class SimpleMovieKnn
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

                int[] repos = uod[userA.ID];
                int[] repoMatches = new int[mod.Length];
                float[] movieWeights = new float[mod.Length];

                for (int rIndx = 0; rIndx < repos.Length; rIndx++)
                {
                    int rID = repos[rIndx];
                    List<int> users = mod[rID];
                    int[] repoCountsA = new int[mod.Length];
                    int[] repoCountsB = new int[mod.Length];

                    foreach (int userB in users)
                    {
                        if (userB == userA.ID) continue;
                        // for every movie in user B
                        foreach(int repo in uod[userB])
                        {
                            if (repo == rID) continue;
                            repoMatches[repo]++;
                            repoCountsA[repo] += mod[repo].Count;
                            repoCountsB[repo] += mod[rID].Count;
                        }
                    }
                    for (int i = 0; i < movieWeights.Length; i++)
                    {
                        if (repoMatches[i] > 0)
                            movieWeights[i] += (repoMatches[i] / (100 + (float)(Math.Sqrt(repoCountsA[i]) * Math.Sqrt(repoCountsB[i])))); 
                    }
                }

                // find x highest
                predictions[usrIndx] = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    int highRepo = 0;
                    float highWeight = 0;

                    for (int j = 0; j < repoMatches.Length; j++)
                    {
                        float weight = movieWeights[j];
                        if (weight > highWeight)
                        {
                            highRepo = j;
                            highWeight = weight;
                        }
                    }
                    movieWeights[highRepo] = 0; // zero out to skip next round
                    predictions[usrIndx][i] = highRepo;
                }
            }
            DataFormatter.OutputPredictions(outPath, td, test, predictions);
        }

    }
}
