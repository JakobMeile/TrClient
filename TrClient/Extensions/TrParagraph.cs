// <copyright file="TrParagraph.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Extensions
{
    using System;
    using System.Text;
    using TrClient.Core;

    public class TrParagraph : IComparable
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public TrTextLine FirstLine;

        public TrParagraphs ParentContainer;
        public TrTextRegion ParentRegion;

        private int parentPageNr;

        public int ParentPageNr
        {
            get
            {
                parentPageNr = ParentRegion.ParentTranscript.ParentPage.PageNr;
                return parentPageNr;
            }
        }

        private int parentRegionNr;

        public int ParentRegionNr
        {
            get
            {
                parentRegionNr = ParentRegion.Number;
                return parentRegionNr;
            }
        }

        private string content;

        public string Content
        {
            get
            {
                content = ToString();
                return content;
            }
        }

        public TrParagraph(int paragraphNumber, TrTextLine startLine)
        {
            Number = paragraphNumber;
            FirstLine = startLine;
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
            TrTextLine currentLine = FirstLine;

            do
            {
                temp = currentLine.ExpandedText;
                if (currentLine.Next != null)
                {
                    if (currentLine.EndsWithHyphen && currentLine.Next.StartsWithSmallLetter)
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }
                }

                sb.Append(temp);
                if (!currentLine.EndsWithHyphen)
                {
                    sb.Append(" ");
                }

                currentLine = currentLine.Next;
            }
            while (currentLine != null);

            temp = sb.ToString();
            while (temp.IndexOf("  ") != -1)
            {
                temp = temp.Replace("  ", " ");
            }

            temp = temp.Replace(" - ", " \u2013 ").Trim();  // en dash
            temp = temp.Replace("- ", "-").Trim();

            return temp;
        }
    }
}
