using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DanishNLP;
using System.Diagnostics;
using System.IO;

namespace DanishNLP
{
    public class clsMonthNames
    {
        private List<string> MonthNames = new List<string>();

        public clsMonthNames()
        {
            string MonthNamesFileName = @"C:\Users\jakob\Dropbox\Code\DanishNLP\DanishNLP\MonthNames.txt";
            string FileLine;

            StreamReader ListFile = new StreamReader(MonthNamesFileName);
            while ((FileLine = ListFile.ReadLine()) != null)
                MonthNames.Add(FileLine);

        }

        public bool CheckWord(string Source)
        {
            return MonthNames.Contains(Source.ToLower());
        }

    }
}
