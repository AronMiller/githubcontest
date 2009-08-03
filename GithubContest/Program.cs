using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GithubContest
{
    class Program
    {
        static void Main(string[] args)
        {
            string repos = @"C:\Users\Aron\Github\download\repos.txt";
            string data = @"C:\Users\Aron\Github\download\data.txt";
            string lang = @"C:\Users\Aron\Github\download\lang.txt";
            string test = @"C:\Users\Aron\Github\download\test.txt";
            string results = @"C:\Users\Aron\Github\GithubContest\GithubContest\results.txt";

            TrainingData trainData = new TrainingData();
            trainData.Load(repos, lang, data);

            TestData testData = new TestData();
            testData.Load(test, trainData);

            LogisticSVD svd = new LogisticSVD();
            svd.Setup(trainData, testData, results);
            for(int epoch = 0; epoch < 200; epoch++)
                svd.Train(1);
            svd.Predict();
        }
    }
}
