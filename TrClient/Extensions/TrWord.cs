// <copyright file="TrWord.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Extensions
{
    using System;
    using System.ComponentModel;
    using DanishNLP;
    using TrClient.Core;

    public class TrWord : IComparable, INotifyPropertyChanged
    {
        public TrWords ParentContainer { get; set; }

        public TrWord Previous { get; set; }

        public TrWord Next { get; set; }

        public int ContextSize = 10;

        private TrPage parentPage;

        public TrPage ParentPage
        {
            get
            {
                parentPage = ParentLine.ParentRegion.ParentTranscript.ParentPage;
                return parentPage;
            }
        }

        private int parentPageNumber;

        public int ParentPageNumber
        {
            get
            {
                parentPageNumber = ParentPage.PageNr;
                return parentPageNumber;
            }
        }

        private TrTextRegion parentRegion;

        public TrTextRegion ParentRegion
        {
            get
            {
                parentRegion = ParentLine.ParentRegion;
                return parentRegion;
            }
        }

        private int parentRegionNumber;

        public int ParentRegionNumber
        {
            get
            {
                parentRegionNumber = ParentRegion.Number;
                return parentRegionNumber;
            }
        }

        public TrTextLine ParentLine { get; set; }

        public int PositionInParentLine { get; set; }

        private int parentLineNumber;

        public int ParentLineNumber
        {
            get
            {
                parentLineNumber = ParentLine.Number;
                return parentLineNumber;
            }
        }

        private int id;

        public int ID
        {
            get
            {
                return id;
            }

            set
            {
                if (id != value)
                {
                    id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string raw;

        public string Raw
        {
            get
            {
                return raw;
            }

            set
            {
                if (raw != value)
                {
                    string old = raw;
                    raw = value;
                    NotifyPropertyChanged("Raw");

                    if (old != null)
                    {
                        ParentLine.Replace(old, raw);
                    }
                }
            }
        }

        private int length;

        public int Length
        {
            get
            {
                length = Raw.Length;
                return length;
            }
        }

        private string stripped;

        public string Stripped
        {
            get
            {
                if (Raw == "&" || Raw == "…" || Raw == "[…]" || Raw == "(…)" || IsOrdinalNumber)
                {
                    stripped = Raw;
                }
                else
                {
                    stripped = ClsLanguageLibrary.StripAll(Raw);
                }

                return stripped;
            }
        }

        private bool endsWithHyphen;

        public bool EndsWithHyphen
        {
            get
            {
                endsWithHyphen = ClsLanguageLibrary.EndsWithHyphen(Raw);
                return endsWithHyphen;
            }
        }

        private bool endsWithAlphaNumeric;

        public bool EndsWithAlphaNumeric
        {
            get
            {
                endsWithAlphaNumeric = ClsLanguageLibrary.EndsWithAlphaNumeric(Raw);
                return endsWithAlphaNumeric;
            }
        }

        private bool startsWithLowerCase;

        public bool StartsWithLowerCase
        {
            get
            {
                startsWithLowerCase = ClsLanguageLibrary.StartsWithLowerCase(Stripped);
                return startsWithLowerCase;
            }
        }

        private bool startsWithUpperCase;

        public bool StartsWithUpperCase
        {
            get
            {
                startsWithUpperCase = ClsLanguageLibrary.StartsWithUpperCase(Stripped);
                return startsWithUpperCase;
            }
        }

        private bool startsWithVowel;

        public bool StartsWithVowel
        {
            get
            {
                startsWithVowel = ClsLanguageLibrary.StartsWithVowel(Stripped);
                return startsWithVowel;
            }
        }

        private bool startsWithConsonant;

        public bool StartsWithConsonant
        {
            get
            {
                startsWithConsonant = ClsLanguageLibrary.StartsWithConsonant(Stripped);
                return startsWithConsonant;
            }
        }

        private bool endsWithVowel;

        public bool EndsWithVowel
        {
            get
            {
                endsWithVowel = ClsLanguageLibrary.IsVowel(Stripped[Length - 1]);
                return endsWithVowel;
            }
        }

        private bool endsWithConsonant;

        public bool EndsWithConsonant
        {
            get
            {
                endsWithConsonant = ClsLanguageLibrary.IsConsonant(Stripped[Length - 1]);
                return endsWithConsonant;
            }
        }

        private bool isAllUpperCase;

        public bool IsAllUpperCase
        {
            // NB: Tester på STRIPPED
            get
            {
                isAllUpperCase = ClsLanguageLibrary.IsAllUpperCase(Stripped);
                return isAllUpperCase;
            }
        }

        private bool isAllLowerCase;

        public bool IsAllLowerCase
        {
            get
            {
                isAllLowerCase = ClsLanguageLibrary.IsAllLowerCase(Stripped);
                return isAllLowerCase;
            }
        }

        private bool isAllConsonant;

        public bool IsAllConsonant
        {
            get
            {
                isAllConsonant = ClsLanguageLibrary.IsAllConsonant(Stripped);
                return isAllConsonant;
            }
        }

        private bool isAllVowel;

        public bool IsAllVowel
        {
            get
            {
                isAllVowel = ClsLanguageLibrary.IsAllVowel(Stripped);
                return isAllVowel;
            }
        }

        private bool isOrdinalNumber;

        public bool IsOrdinalNumber
        {
            get
            {
                // her bruges RAW, fordi det afsluttende punktum er centralt!
                isOrdinalNumber = ClsLanguageLibrary.IsOrdinalNumber(Raw);
                return isOrdinalNumber;
            }
        }

        private bool endsWithPeriodColonExclamationQuestionMark;

        public bool EndsWithPeriodColonExclamationQuestionMark
        {
            get
            {
                // NB: Tester på RAW - for ellers er disse mærker jo fjernet...
                endsWithPeriodColonExclamationQuestionMark = ClsLanguageLibrary.EndsWithPeriodColonExclamationQuestionMark(Raw);
                return endsWithPeriodColonExclamationQuestionMark;
            }
        }

        private bool endsWithComma;

        public bool EndsWithComma
        {
            get
            {
                // NB: Tester på RAW - for ellers er disse mærker jo fjernet...
                endsWithComma = ClsLanguageLibrary.EndsWithComma(Raw);
                return endsWithComma;
            }
        }

        //private bool _isMonthName;
        //public bool IsMonthName
        //{
        //    get
        //    {
        //        _isMonthName = MonthNameList.Contains(Stripped.ToLower());
        //        return _isMonthName;
        //    }
        //}
        private bool endsQuotation;

        public bool EndsQuotation
        {
            get
            {
                endsQuotation = ClsLanguageLibrary.EndsQuotation(Raw);
                return endsQuotation;
            }
        }

        private bool startsQuotation;

        public bool StartsQuotation
        {
            get
            {
                startsQuotation = ClsLanguageLibrary.StartsQuotation(Raw);
                return startsQuotation;
            }
        }

        //private bool _endsSentence;
        //public bool EndsSentence
        //{
        //    get
        //    {
        //        _endsSentence = EndsWithPeriodColonExclamationQuestionMark && !IsAbbreviation;
        //        return _endsSentence;
        //    }

        //}

        //private bool _startsSentence;
        //public bool StartsSentence
        //{
        //    get
        //    {
        //        if (Previous != null)
        //            _startsSentence = Previous.EndsSentence && StartsWithUpperCase;
        //        else
        //            _startsSentence = StartsWithUpperCase;
        //        return _startsSentence;
        //    }
        //}

        //private bool _isCommonSentenceStarter;
        //public bool IsCommonSentenceStarter
        //{
        //    get
        //    {
        //        _isCommonSentenceStarter = SentenceStarter.CheckWord(Stripped);
        //        return _isCommonSentenceStarter;
        //    }
        //}

        //private bool _isCommonTitle;
        //public bool IsCommonTitle
        //{
        //    get
        //    {
        //        _isCommonTitle = Titles.CheckWord(Stripped);
        //        return _isCommonTitle;
        //    }
        //}

        //private bool _isAbbreviation;
        //public bool IsAbbreviation
        //{
        //    get
        //    {
        //        _isAbbreviation = Abbreviations.CheckWord(Stripped);
        //        return _isAbbreviation;
        //    }
        //}
        private string context;

        public string Context
        {
            get
            {
                string temp;
                if (ID > ContextSize && ID < (ParentContainer.Count - ContextSize))
                {
                    TrWord nextWord = Next;
                    TrWord previousWord = Previous;
                    string after = string.Empty;
                    string before = string.Empty;

                    for (int i = 1; i <= ContextSize; i++)
                    {
                        after = after + " " + nextWord.Stripped; //    Raw;
                        nextWord = nextWord.Next;

                        before = previousWord.Stripped + " " + before;
                        previousWord = previousWord.Previous;

                        // Debug.WriteLine($"Before: {Before} - After: {After}");
                    }

                    temp = before + " " + Stripped + " " + after;
                }
                else
                {
                    temp = Raw;
                }

                context = temp.Replace("  ", " ").Trim();
                return context;
            }
        }

        public bool EndsWith(string ending)
        {
            return Raw.EndsWith(ending);
        }

        // Constructor
        public TrWord(string text, TrTextLine sourceLine)   // , int Position
        {
            Raw = text.Trim();
            ParentLine = sourceLine;

            // PositionInParentLine = Position;
        }

        public int CompareTo(object obj)
        {
            var c = obj as TrWord;
            return Raw.CompareTo(c.Raw);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
