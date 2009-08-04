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
        float reg = 0f;//.015f;

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
            featureCount = 10;
            

            // model params
            Random r = new Random();
            userBias = new float[userCount];
            for (int i = 0; i < userCount; i++)
                userBias[i] = -10;
            repoBias = new float[repoCount];
            for (int i = 0; i < userCount; i++)
                repoBias[i] = -10;
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
            
            for (int epoch = 0; epoch < epochMax; epoch++)
            {
                for (int i = 0; i < userCount; i++)
                {
                    for (int f = 0; f < featureCount; f++)
                    {
                        userFeatures[i][f] = -.1f + .001f * r.Next(200);
                    }
                }

                float trRate = .01f;
                float totalErrPos = 0f;
                float totalErrNeg = 0f;
                int countPos = 0;
                int countNeg = 0;
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
 
                        float[] rf = repoFeatures[repo];

                        float pred = userBias[user] + repoBias[repo];
                        for (int f = 0; f < featureCount; f++)
                        {
                            pred += uf[f] * rf[f];
                        }

                        pred = (float)(1f / (1f + Math.Exp(-pred)));
                        float err = 1f - pred;
                        totalErrPos += err * err;

                       // userBias[user] += trRate * err;
                       // repoBias[repo] += trRate * err;

                        for (int f = 0; f < featureCount; f++)
                        {
                            uf[f] += trRate * (err * rf[f] - reg * uf[f]);
                            rf[f] += trRate * (err * uf[f] - reg * rf[f]);
                        }
                        countPos++;
                    }

                    
              /*      // random negative selections
                    for (int j = 0; j < repos.Length * 10; j++)
                    {
                        int repo = r.Next(repoCount); 

                        float[] rf = repoFeatures[repo];

                        float pred = userBias[user] + repoBias[repo];
                        for (int f = 0; f < featureCount; f++)
                        {
                            pred += uf[f] * rf[f];
                        }

                        //pred = (float)(1f / (1f + Math.Exp(-pred)));
                        float err = 0f - pred;
                        totalErrNeg += err * err;

                        userBias[user] += trRate * err;
                        repoBias[repo] += trRate * err;

                        for (int f = 0; f < featureCount; f++)
                        {
                            uf[f] += trRate * (err * rf[f] - reg * uf[f]);
                            rf[f] += trRate * (err * uf[f] - reg * rf[f]);
                        }
                        countNeg++;
                    }*/
                }
                totalErrPos = (float)Math.Sqrt(totalErrPos / countPos);
                totalErrNeg = (float)Math.Sqrt(totalErrNeg / countPos);
                Console.WriteLine(totalErrPos + "," + totalErrNeg); 
            }
        }

        public void Predict()
        {
            //StreamWriter sw = new StreamWriter(File.OpenWrite(results));
            
            // predict all repos and sort
            List<IntFloat> sortMe = new List<IntFloat>();
            int[][] outPredictions = new int[test.Users.Count][];
            int usrCnt = 0;
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
                    if (pred > .3f)
                    {
                        IntFloat sort = new IntFloat();
                        sort.Int = repo;
                        sort.Float = pred;
                        sortMe.Add(sort);
                    }
                }

                sortMe.Sort();

                int cnt = 0;
                int predictions = (sortMe.Count < 10 ? sortMe.Count : 10);
                int[] predictList = new int[predictions];
                foreach (IntFloat sort in sortMe)
                {
                    Repository r = usr.Repo.GetByInternalID(sort.Int);
                    if (r != null)
                        continue;
                    else
                    {
                        predictList[cnt] = td.Repositories.GetByInternalID(sort.Int).ExternalID;
                        cnt++;
                        if (cnt == predictions) break;
                    }
                }
                
                outPredictions[usrCnt] = predictList;

                // Console Output
                string outStr = usr.ExternalID + ":";
                for (int i = 0; i < predictions; i++)
                {
                    outStr += predictList[i];
                    if (i < predictions - 1) outStr += ",";
                }

                DataFormatter.OutputPredictions(results, td, test, outPredictions);
                Console.WriteLine(outStr);
                //sw.WriteLine(outStr);
                sortMe.Clear();
            }
            //sw.Close();
        }
    }
}
