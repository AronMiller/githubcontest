using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GithubContest
{
    /// <summary>
    /// I'd like whatever license requires acknowledgement of where the idea came from that's about it. thx.
    /// </summary>
    public class LogisticSVD
    {

        TrainingData td;
        TestData test;
        string results;
        int[][] uod;
        int userCount;
        int repoCount;
        float reg = .015f;

        int featureCount;
        float[] userBias; 
        float[] repoBias; 
        float[][] userFeatures;
        float[][] repoFeatures;

        Random r = new Random(0);


        public void Setup(TrainingData td, TestData test, string results)
        {
            this.td = td;
            this.test = test;
            this.results = results;
            // model training data
            uod = DataFormatter.GetUserOrderRepositories(td);
            userCount = td.Users.Count;
            repoCount = td.Repositories.Count;

            // model hypers
            featureCount = 1;
            

            // model params
            Random r = new Random();
            userBias = new float[userCount];
            repoBias = new float[repoCount];
            userFeatures = new float[userCount][];
            for (int i = 0; i < userCount; i++)
            {
                userFeatures[i] = new float[featureCount];
                for (int f = 0; f < featureCount; f++)
                {
                    userFeatures[i][f] = -.1f + .001f * r.Next(200);
                }
            }

            repoFeatures = new float[repoCount][];
            for (int i = 0; i < repoCount; i++)
            {
                repoFeatures[i] = new float[featureCount];
                for (int f = 0; f < featureCount; f++)
                {
                    repoFeatures[i][f] = -.1f + .001f * r.Next(200);
                }
            }
        }
        public void Train(int epochMax)
        {
            // model train
            int count = 0;
            for (int epoch = 0; epoch < epochMax; epoch++)
            {
                float trRate = .01f * (float)Math.Pow(.9, epoch);
                float totalErr = 0f;
                for (int user = 0; user < userCount; user++)
                {
                    int[] repos = uod[user];
                    float[] uf = userFeatures[user];
                    //int usrPos = 0;
                   // int usrRepo = repos[usrPos];
                    
                    // positive selections
                    for(int j = 0; j < repos.Length; j++)
                    {
                        int repo = repos[j];
                    //for (int repo = 0; repo < repoCount; repo++)
                    //{
                        /*float targValue;
                        if (repo < usrRepo)
                        {
                            targValue = 0;
                            continue;
                        }
                        else if (repo == usrRepo)
                        {
                            targValue = 1;
                            usrPos++;
                            if (usrPos < repos.Length)
                            {
                                usrRepo = repos[usrPos];
                            }
                        }
                        else
                        {
                            targValue = 0;
                            continue;
                        }*/

                        float[] rf = repoFeatures[repo];

                        float pred = userBias[user] + repoBias[repo];
                        for (int f = 0; f < featureCount; f++)
                        {
                            pred += uf[f] * rf[f];
                        }

                        pred = (float)(1f / (1f + Math.Exp(-pred)));
                        float err = 1f - pred;
                        totalErr += err * err;

                        userBias[user] += trRate * err;
                        repoBias[repo] += trRate * err;

                        for (int f = 0; f < featureCount; f++)
                        {
                            uf[f] += trRate * (err * rf[f] - reg * uf[f]);
                            rf[f] += trRate * (err * uf[f] - reg * rf[f]);
                        }
                        count++;
                    }

                    // random negative selections
                    for (int j = 0; j < 100; j++)
                    {
                        int repo = r.Next(repoCount); 

                        float[] rf = repoFeatures[repo];

                        float pred = userBias[user] + repoBias[repo];
                        for (int f = 0; f < featureCount; f++)
                        {
                            pred += uf[f] * rf[f];
                        }

                        pred = (float)(1f / (1f + Math.Exp(-pred)));
                        float err = 0f - pred;
                        totalErr += err * err;

                        userBias[user] += trRate * err;
                        repoBias[repo] += trRate * err;

                        for (int f = 0; f < featureCount; f++)
                        {
                            uf[f] += trRate * (err * rf[f] - reg * uf[f]);
                            rf[f] += trRate * (err * uf[f] - reg * rf[f]);
                        }
                        count++;
                    }
                }
                Console.WriteLine(Math.Sqrt(totalErr / count));
            }
        }

        public void Predict()
        {
            StreamWriter sw = new StreamWriter(File.OpenWrite(results));
            
            // predict all repos and sort
            List<IntFloat> sortMe = new List<IntFloat>();
            foreach (User usr in test.Users)
            {
                float[] uf = userFeatures[usr.ID];
                //int usrPos = 0;
                // int usrRepo = repos[usrPos];

                for (int repo = 0; repo < repoCount; repo++)
                {
                    float[] rf = repoFeatures[repo];

                    float pred = userBias[usr.ID] + repoBias[repo];
                    for (int f = 0; f < featureCount; f++)
                    {
                        pred += uf[f] * rf[f];
                    }

                    //pred = (float)(1f / (1f + Math.Exp(-pred)));
                    IntFloat sort = new IntFloat();
                    sort.Int = repo;
                    sort.Float = -pred;
                    sortMe.Add(sort); 
                }

                sortMe.Sort();

                int cnt = 0;
                int[] predictList = new int[10];
                foreach (IntFloat sort in sortMe)
                {
                    if (usr.Repo.GetByInternalID(sort.Int) != null)
                        continue;
                    else
                    {
                        predictList[cnt] = sort.Int;
                        cnt++;
                        if (cnt == 10) break;
                    }
                }

                // print to file
                string outStr = usr.ID + ":";
                for (int i = 0; i < 10; i++)
                {
                    outStr += predictList[i];
                    if (i < 9) outStr += ",";
                }
                sw.WriteLine(outStr);
                sortMe.Clear();
            }
            sw.Close();
        }
    }
}
