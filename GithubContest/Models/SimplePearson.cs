using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest.Models
{
    public class SimplePearson
    {
        TrainingData trainData;
        TestData testData;

        int[][] uod;
        float[][] userCorr;

        public void Setup(TrainingData trainData, TestData testData)
        {
            this.trainData = trainData;
            this.testData = testData;

            uod = DataFormatter.GetUserOrderRepositories(trainData);
        }

        public void Run()
        {

        }

        public void Predict()
        {
        }
    }
}
