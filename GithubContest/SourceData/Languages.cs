using System;
using System.Collections.Generic;
using System.Text;

namespace GithubContest
{
    public class Languages: List<Language>
    {
        public Language GetLanguageOrAddNew(string langName)
        {
            Language selectLang = null;
            foreach (Language lang1 in this)
                if (lang1.Name == langName)
                {
                    selectLang = lang1;
                    break;
                }
            if (selectLang == null)
            {
                selectLang = new Language();
                selectLang.Name = langName;
                this.Add(selectLang);
            }
            return selectLang;
        }
    }
}
