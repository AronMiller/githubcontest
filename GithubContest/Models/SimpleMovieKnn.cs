using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class SimpleMovieKnn
    {

        public Dictionary<int,float>[] GetReposCorrs(TrainingData td, int[][] uod, List<int>[] mod)
        {
            Dictionary<int, float>[] retVal = new Dictionary<int, float>[td.Repositories.Count];
            for (int i = 0; i < retVal.Length; i++) retVal[i] = new Dictionary<int, float>();
           
            int[] matches = new int[td.Repositories.Count];

            for (int reposA = 0; reposA < td.Repositories.Count; reposA++)
            {
                List<int> users = mod[reposA];
                
                foreach (int userA in users)
                {
                    int[] reposBs = uod[userA];
                    foreach(int reposB in reposBs)
                    {
                        if(reposB == reposA) continue;
                        matches[reposB]++;
                    }
                }
                for (int i = 0; i < td.Repositories.Count; i++)
                {
                    int matchCount = matches[i];
                    if (matchCount > 0)
                    {
                        float weight = matches[i] / (float)(50 + Math.Sqrt(mod[reposA].Count) * Math.Sqrt(mod[i].Count));
                        retVal[reposA][i] = weight;
                        matches[i] = 0;
                    }
                }
            }
            return retVal;
        }
        // for each user in test
        // find similar users, count up 
        public int[][] Run2(TrainingData td, TestData test, string outPath)
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
            Dictionary<int, float>[] corrs = GetReposCorrs(td, uod, mod);
            int[][] predictions = new int[test.Users.Count][];
            Repository[] repoList = td.Repositories.GetList();
            for (int usrIndx = 0; usrIndx < test.Users.Count; usrIndx++)
            {
                User userA = test.Users[usrIndx];
                float[] weights = new float[td.Repositories.Count];
                foreach(int repos in uod[userA.ID])
                {
                    foreach(KeyValuePair<int,float> kvp in corrs[repos])
                    {
                        if (kvp.Key == repos) continue;
                        weights[kvp.Key] += kvp.Value; 
                    }
                }
                // find x highest
                predictions[usrIndx] = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    int highRepo = 0;
                    float highWeight = 0;

                    for (int j = 0; j < weights.Length; j++)
                    {
                        float weight = weights[j];
                        if (weight > highWeight)
                        {
                            highRepo = j;
                            highWeight = weight;
                        }
                    }
                    weights[highRepo] = 0; // zero out to skip next round
                    predictions[usrIndx][i] = highRepo;
                }
            }
            return predictions;

        }

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
           // Dictionary<int, float>[] corrs = GetReposCorrs(td, uod, mod);

            int[][] predictions = new int[test.Users.Count][];

            for (int usrIndx = 0; usrIndx < test.Users.Count; usrIndx++)
            {
                User userA = test.Users[usrIndx];

                int[] repos = uod[userA.ID];
                int[] repoMatches = new int[mod.Length];
                float[] movieWeights = new float[mod.Length];
                int[] repoCountsA = new int[mod.Length];
                //int[] repoCountsB = new int[mod.Length];

                for (int rIndx = 0; rIndx < repos.Length; rIndx++)
                {
                    int rID = repos[rIndx];
                    List<int> users = mod[rID];

                    foreach (int userB in users)
                    {
                        if (userB == userA.ID) continue;
                        // for every movie in user B
                        
                        foreach(int repo in uod[userB])
                        {
                            if (repo == rID) continue;
                            repoMatches[repo]++;
                        }
                    }
                }
                
                for (int i = 0; i < movieWeights.Length; i++)
                {
                    if (repoMatches[i] > 0)
                        movieWeights[i] = repoMatches[i]; // (repoMatches[i] / (1000 + (float)(Math.Sqrt(repoCountsA[i]) * Math.Sqrt(repoCountsB[i]))));
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
            return predictions;
        }

    }
}
