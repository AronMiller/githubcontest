﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GithubContest
{
    class Program
    {
        private static void QuickFix(TrainingData trainData)
        {
            string results = @"C:\Users\Aron\Github\GithubContest\results.txt";
            string[] allLines = File.ReadAllLines(results);
            StreamWriter sw = new StreamWriter(File.OpenWrite(results + ".bak"));

            foreach (string s in allLines)
            {
                int usrID = int.Parse(s.Substring(0, s.IndexOf(':')));
                User u = trainData.Users[usrID];
                string outStr = u.ExternalID.ToString() + s.Substring(s.IndexOf(':'));
                sw.WriteLine(outStr);
            }
            sw.Close();
        }

        static void Main(string[] args)
        {
            string repos = @"C:\Users\Aron\Github\download\repos.txt";
            string data = @"C:\Users\Aron\Github\download\data.txt";
            string lang = @"C:\Users\Aron\Github\download\lang.txt";
            string test = @"C:\Users\Aron\Github\download\test.txt";
            string results = @"C:\Users\Aron\Github\GithubContest\results.txt";


            

            TrainingData trainData = new TrainingData();
            trainData.Load(repos, lang, data);
            
            TestData testData = new TestData();
            testData.Load(test, trainData);

            SimpleMovieKnn smknn = new SimpleMovieKnn();
            int[][] predictions1 = smknn.Run2(trainData, testData, results);
            
            SimpleCounterModel scm = new SimpleCounterModel();
            int[][] predictions2 = scm.Run(trainData, testData, results);
            
            

            // combine
            int[][] blend = new int[testData.Users.Count][];
            for (int i = 0; i < blend.Length; i++)
            {
                blend[i] = new int[10];
                for (int j = 0; j < 5; j++)
                {
                    blend[i][j] = predictions1[i][j];
                }
                int cnt = 5;
                foreach (int j in predictions2[i])
                {
                    if (!blend[i].Contains<int>(j))
                    {
                        blend[i][cnt] = j;
                        cnt++;
                        if (cnt == 10) break;
                    }
                }
            }
            DataFormatter.OutputPredictions(results, trainData, testData, blend);

            //QuickFix(trainData);
            /*
            LogisticSVD svd = new LogisticSVD();
            svd.Setup(trainData, testData, results);
            for(int epoch = 0; epoch < 500; epoch++)
                svd.Train(1);
            svd.Predict();*/
        }
    }
}
