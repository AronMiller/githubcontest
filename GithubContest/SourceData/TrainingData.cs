using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GithubContest
{
    public class TrainingData
    {
        public Repositories Repositories = new Repositories(); 
        public Languages Languages = new Languages();
        public Users Users = new Users();

        private void LoadRepos(string repos)
        {
            string[] lines = File.ReadAllLines(repos);
            int id = 0;
            foreach (string s in lines)
            {
                int externID = int.Parse(s.Substring(0, s.IndexOf(':')));
                string[] split = s.Split(',');
                Repository r = new Repository();
                r.ID = id++;
                r.ExternalID = externID;
                r.Name = split[0];
                r.DateCreated = DateTime.Parse(split[1]);
                Repositories.AddRepository(r);
            }
        }       

        private void LoadLangs(string lang)
        {
            string[] lines = File.ReadAllLines(lang);
            foreach (string s in lines)
            {
                string[] split = s.Split(':');
                string strID = split[0]; 
                int externID = int.Parse(strID);
                Repository r = Repositories.GetByExternID(externID);
                if (r == null) continue;
                
                string[] split2 = split[1].Split(',');
                int totalLoc = 0;
                foreach (string l in split2)
                {
                    string[] split3 = l.Split(';');
                    string langName = split3[0];
                    int loc = int.Parse(split3[1]);
                    // check that language is defined
                    Language selectLang = Languages.GetLanguageOrAddNew(langName);
                    // add repo language item
                    RepoLanguage rl = new RepoLanguage();
                    rl.Language = selectLang;
                    rl.LOC = loc;
                    totalLoc += loc;
                    r.Languages.Add(rl);
                }
                foreach (RepoLanguage rl in r.Languages)
                {
                    rl.ShareLOC = rl.LOC / (float)totalLoc;
                }
            }
        }

        private void LoadData(string data)
        {
            string[] lines = File.ReadAllLines(data);
            foreach (string s in lines)
            {
                string[] split = s.Split(':');
                int usrID = int.Parse(split[0]);
                int repoID = int.Parse(split[1]);
                User u = Users.GetOrAddNew(usrID);
                u.Repo.AddRepository(Repositories.GetByExternID(repoID));
            }
        }

        public void Load(string repos, string lang, string data)
        {
            LoadRepos(repos);
            LoadLangs(lang);
            LoadData(data);
        }
    }
}
