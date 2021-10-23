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



namespace TrClient.Extensions
{
    public class TrParagraph : IComparable
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public TrTextLine FirstLine;

        public TrParagraphs ParentContainer;
        public TrRegion_Text ParentRegion;

        private int _parentPageNr;
        public int ParentPageNr
        {
            get
            {
                _parentPageNr = ParentRegion.ParentTranscript.ParentPage.PageNr;
                return _parentPageNr;
            }
        }

        private int _parentRegionNr;
        public int ParentRegionNr
        {
            get
            {
                _parentRegionNr = ParentRegion.Number;
                return _parentRegionNr;
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                _content = ToString();
                return _content;
            }
        }

        public TrParagraph(int ParagraphNumber, TrTextLine StartLine)
        {
            Number = ParagraphNumber;
            FirstLine = StartLine;
            Name = FirstLine.StructuralTagValue;
        }

        public int CompareTo(object obj)
        {
            var paragraph = obj as TrParagraph;
            return Number.CompareTo(paragraph.Number);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string temp;
            TrTextLine CurrentLine = FirstLine;

            do
            {
                temp = CurrentLine.ExpandedText;
                if (CurrentLine.Next != null)
                    if (CurrentLine.EndsWithHyphen && CurrentLine.Next.StartsWithSmallLetter)
                        temp = temp.Substring(0, temp.Length - 1);
                sb.Append(temp);
                if (!CurrentLine.EndsWithHyphen)
                    sb.Append(" ");
                CurrentLine = CurrentLine.Next;
            }
            while (CurrentLine != null);

            temp = sb.ToString();
            while (temp.IndexOf("  ") != -1)
                temp = temp.Replace("  ", " ");

            temp = temp.Replace(" - ", " \u2013 ").Trim();  // en dash
            temp = temp.Replace("- ", "-").Trim();

            return temp;
        }
    }
}
