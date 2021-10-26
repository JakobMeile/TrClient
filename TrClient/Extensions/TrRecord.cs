// <copyright file="TrRecord.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Extensions
{
    using System.Text.RegularExpressions;
    using TrClient.Libraries;

    public class TrRecord
    {
        public string Name;
        public string Date;
        public string Metadata;

        public string Source;
        public int SourcePageNumber;
        public int SourceLineNumber;

        public TrRecord(string pName, string pDate, string pMetadata, string pSource, int pSourcePageNumber, int pSourceLineNumber)
        {
            Name = pName;

            // her fjerner vi lige alt muligt snask efter datoen
            Regex dates = new Regex(@"\d{4}-\d{2}(-\d{2})?");
            MatchCollection dateMatches = dates.Matches(pDate);
            if (dateMatches.Count == 1)
            {
                Date = dateMatches[0].Value;
            }
            else
            {
                Date = pDate;
            }

            Metadata = pMetadata;
            Source = pSource;
            SourcePageNumber = pSourcePageNumber;
            SourceLineNumber = pSourceLineNumber;
        }

        public override string ToString()
        {
            string temp = string.Empty;
            char delimiter = TrLibrary.CSVDelimiter;

            temp = Name + delimiter + Date + delimiter + Source + delimiter + SourcePageNumber.ToString() + delimiter + SourceLineNumber.ToString() + delimiter + Metadata;

            //Regex Numbers = new Regex(@"\d+\p{L}?");
            //string Stripped = clsLanguageLibrary.StripAll(Name);
            //string NewName = "";
            //MatchCollection NumberMatches = Numbers.Matches(Stripped);
            //if (NumberMatches.Count > 0)
            //{
            //    NewName = ExtractRecordName(NumberMatches[0].Value);
            //}
            //if (NumberMatches.Count > 1)
            //{
            //    Metadata = Metadata + Delimiter + "Alt.: " + ExtractRecordName(NumberMatches[1].Value);
            //    //Debug.Print($"ERROR: More than one record: page {SourcePageNumber}, line {SourceLineNumber}");
            //}

            //Temp = NewName + Delimiter + Date + Delimiter + Source + Delimiter + SourcePageNumber.ToString() + Delimiter + SourceLineNumber.ToString() + Delimiter + Metadata;
            return temp;
        }
    }
}
