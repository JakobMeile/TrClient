using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

using DanishNLP;

namespace TrClient.Extensions
{
    public class TrWord : IComparable, INotifyPropertyChanged
    {
        public TrWords ParentContainer { get; set; }
        public TrWord Previous { get; set; }
        public TrWord Next { get; set; }

        public int ContextSize = 10;

        private TrPage _parentPage;
        public TrPage ParentPage
        {
            get
            {
                _parentPage = ParentLine.ParentRegion.ParentTranscript.ParentPage;
                return _parentPage;
            }
        }

        private int _parentPageNumber;
        public int ParentPageNumber
        {
            get
            {
                _parentPageNumber = ParentPage.PageNr;
                return _parentPageNumber;
            }
        }

        private TrRegion_Text _parentRegion;
        public TrRegion_Text ParentRegion
        {
            get
            {
                _parentRegion = ParentLine.ParentRegion;
                return _parentRegion;
            }
        }

        private int _parentRegionNumber;
        public int ParentRegionNumber
        {
            get
            {
                _parentRegionNumber = ParentRegion.Number;
                return _parentRegionNumber;
            }
        }

        public TrTextLine ParentLine { get; set; }

        public int PositionInParentLine { get; set; }

        private int _parentLineNumber;
        public int ParentLineNumber
        {
            get
            {
                _parentLineNumber = ParentLine.Number;
                return _parentLineNumber;
            }
        }
               
        private int _id;
        public int ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _raw;
        public string Raw
        {
            get { return _raw; }
            set
            {
                if (_raw != value)
                {
                    string Old = _raw;
                    _raw = value;
                    NotifyPropertyChanged("Raw");

                    if (Old != null)
                    {
                        ParentLine.Replace(Old, _raw);
                    }

                }
            }
        }
        
        private int _length;
        public int Length
        {
            get
            {
                _length = Raw.Length;
                return _length;
            }
        }
        
        private string _stripped;
        public string Stripped
        {
            get
            {
                if (Raw == "&" || Raw == "…" || Raw == "[…]" || Raw == "(…)" || IsOrdinalNumber)
                    _stripped = Raw;
                else
                    _stripped = clsLanguageLibrary.StripAll(Raw);
                return _stripped;
            }
        }


        private bool _endsWithHyphen;
        public bool EndsWithHyphen
        {
            get
            {
                _endsWithHyphen = clsLanguageLibrary.EndsWithHyphen(Raw);
                return _endsWithHyphen;
            }
        }

        private bool _endsWithAlphaNumeric;
        public bool EndsWithAlphaNumeric
        {
            get
            {
                _endsWithAlphaNumeric = clsLanguageLibrary.EndsWithAlphaNumeric(Raw);
                return _endsWithAlphaNumeric;
            }
        }


        private bool _startsWithLowerCase;
        public bool StartsWithLowerCase
        {
            get
            {
                _startsWithLowerCase = clsLanguageLibrary.StartsWithLowerCase(Stripped);
                return _startsWithLowerCase;
            }
        }

        private bool _startsWithUpperCase;
        public bool StartsWithUpperCase
        {
            get
            {
                _startsWithUpperCase = clsLanguageLibrary.StartsWithUpperCase(Stripped);
                return _startsWithUpperCase;
            }
        }

        private bool _startsWithVowel;
        public bool StartsWithVowel
        {
            get
            {
                _startsWithVowel = clsLanguageLibrary.StartsWithVowel(Stripped);
                return _startsWithVowel;
            }
        }

        private bool _startsWithConsonant;
        public bool StartsWithConsonant
        {
            get
            {
                _startsWithConsonant = clsLanguageLibrary.StartsWithConsonant(Stripped);
                return _startsWithConsonant;
            }
        }

        private bool _endsWithVowel;
        public bool EndsWithVowel
        {
            get
            {
                _endsWithVowel = clsLanguageLibrary.IsVowel(Stripped[Length - 1]);
                return _endsWithVowel;
            }
        }

        private bool _endsWithConsonant;
        public bool EndsWithConsonant
        {
            get
            {
                _endsWithConsonant = clsLanguageLibrary.IsConsonant(Stripped[Length - 1]);
                return _endsWithConsonant;
            }
        }

        private bool _isAllUpperCase;
        public bool IsAllUpperCase
        {
            // NB: Tester på STRIPPED 
            get
            {
                _isAllUpperCase = clsLanguageLibrary.IsAllUpperCase(Stripped);
                return _isAllUpperCase;
            }
        }

        private bool _isAllLowerCase;
        public bool IsAllLowerCase
        {
            get
            {
                _isAllLowerCase = clsLanguageLibrary.IsAllLowerCase(Stripped);
                return _isAllLowerCase;
            }
        }

        private bool _isAllConsonant;
        public bool IsAllConsonant
        {
            get
            {
                _isAllConsonant = clsLanguageLibrary.IsAllConsonant(Stripped);
                return _isAllConsonant;
            }
        }

        private bool _isAllVowel;
        public bool IsAllVowel
        {
            get
            {
                _isAllVowel = clsLanguageLibrary.IsAllVowel(Stripped);
                return _isAllVowel;
            }
        }


        private bool _isOrdinalNumber;
        public bool IsOrdinalNumber
        {
            get
            {
                // her bruges RAW, fordi det afsluttende punktum er centralt!
                _isOrdinalNumber = clsLanguageLibrary.IsOrdinalNumber(Raw);
                return _isOrdinalNumber;
            }
        }

        private bool _endsWithPeriodColonExclamationQuestionMark;
        public bool EndsWithPeriodColonExclamationQuestionMark
        {
            get
            {
                // NB: Tester på RAW - for ellers er disse mærker jo fjernet...
                _endsWithPeriodColonExclamationQuestionMark = clsLanguageLibrary.EndsWithPeriodColonExclamationQuestionMark(Raw);
                return _endsWithPeriodColonExclamationQuestionMark;
            }
        }

        private bool _endsWithComma;
        public bool EndsWithComma
        {
            get
            {
                // NB: Tester på RAW - for ellers er disse mærker jo fjernet...
                _endsWithComma = clsLanguageLibrary.EndsWithComma(Raw);
                return _endsWithComma;
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


        private bool _endsQuotation;
        public bool EndsQuotation
        {
            get
            {
                _endsQuotation = clsLanguageLibrary.EndsQuotation(Raw);
                return _endsQuotation;
            }
        }

        private bool _startsQuotation;
        public bool StartsQuotation
        {
            get
            {
                _startsQuotation = clsLanguageLibrary.StartsQuotation(Raw);
                return _startsQuotation;
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


        private string _context;
        public string Context
        {
            get
            {
                string Temp;
                if (ID > ContextSize && ID < (ParentContainer.Count - ContextSize))
                {
                    TrWord NextWord = Next;
                    TrWord PreviousWord = Previous;
                    string After = "";
                    string Before = "";

                    for (int i = 1; i <= ContextSize; i++)
                    {
                        After = After + " " + NextWord.Stripped; //    Raw;
                        NextWord = NextWord.Next;

                        Before = PreviousWord.Stripped + " " + Before;
                        PreviousWord = PreviousWord.Previous;
                        // Debug.WriteLine($"Before: {Before} - After: {After}");
                    }
                    Temp = Before + " " + Stripped + " " + After;
                }
                else
                    Temp = Raw;
                _context = Temp.Replace("  ", " ").Trim();
                return _context;
            }
        }
        
        public bool EndsWith(string Ending)
        {
            return Raw.EndsWith(Ending);
        }
        

        // Constructor
        public TrWord(string Text, TrTextLine SourceLine)   // , int Position
        {
            Raw = Text.Trim();
            ParentLine = SourceLine;
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
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


    }
}
