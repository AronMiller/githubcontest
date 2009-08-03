using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GithubContest
{
    public class TestData
    {
        public Users Users = new Users();

        public void Load(string test, TrainingData td)
        {
            string[] lines = File.ReadAllLines(test);
            foreach(string s in lines)
            {
                Users.Add(td.Users.GetOrAddNew(int.Parse(s)));
            }
        }
    }
}
