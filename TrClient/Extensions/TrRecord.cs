using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using System.IO;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

using System.Text.RegularExpressions;
using DanishNLP;

namespace TrClient.Extensions
{
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
            Regex Dates = new Regex(@"\d{4}-\d{2}(-\d{2})?");
            MatchCollection DateMatches = Dates.Matches(pDate);
            if (DateMatches.Count == 1)
            {
                Date = DateMatches[0].Value;
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
            string Temp = "";
            char Delimiter = TrLibrary.CSV_Delimiter;

            Temp = Name + Delimiter + Date + Delimiter + Source + Delimiter + SourcePageNumber.ToString() + Delimiter + SourceLineNumber.ToString() + Delimiter + Metadata;


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

            return Temp;
        }

    }
}
