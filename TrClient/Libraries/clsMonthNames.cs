// <copyright file="clsMonthNames.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DanishNLP
{
    using System.Collections.Generic;
    using System.IO;

    public class ClsMonthNames
    {
        private List<string> monthNames = new List<string>();

        public ClsMonthNames()
        {
            string monthNamesFileName = "Resources/MonthNames.txt"; // @"C:\Users\jakob\Dropbox\Code\DanishNLP\DanishNLP\MonthNames.txt";
            string fileLine;

            StreamReader listFile = new StreamReader(monthNamesFileName);
            while ((fileLine = listFile.ReadLine()) != null)
            {
                monthNames.Add(fileLine);
            }
        }

        public bool CheckWord(string source)
        {
            return monthNames.Contains(source.ToLower());
        }
    }
}
