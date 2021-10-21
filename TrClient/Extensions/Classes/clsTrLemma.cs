using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using TrClient;
using DanishNLP;

namespace TrClient
{
    public class clsTrLemma : IComparable, INotifyPropertyChanged
    {
        public clsTrLemmas ParentContainer;

        public List<clsTrWord> Occurrences = new List<clsTrWord>();

        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;

                    foreach (clsTrWord Word in Occurrences)
                        Word.Raw = _content;

                    NotifyPropertyChanged("Content");
                    NotifyPropertyChanged("Stripped");
                    NotifyPropertyChanged("ErrorIndex");
                    HasChanged = true;
                }
            }
        }

        private string _stripped;
        public string Stripped
        {
            get
            {
                if (Content == "&" || Content == "…" || Content == "[…]" || Content == "(…)" || IsOrdinalNumber)
                    _stripped = Content;
                else
                    _stripped = clsLanguageLibrary.StripAll(Content);
                return _stripped;
            }
        }


        private bool _isOrdinalNumber;
        public bool IsOrdinalNumber
        {
            get
            {
                // her bruges RAW, fordi det afsluttende punktum er centralt!
                _isOrdinalNumber = clsLanguageLibrary.IsOrdinalNumber(Content);
                return _isOrdinalNumber;
            }
        }

        public bool MarkToDeletion = false;

        private int _occurrenceCount;
        public int OccurrenceCount
        {
            get
            {
                _occurrenceCount = Occurrences.Count;
                return _occurrenceCount;
            }
        }

        private bool _hasChanged = false;
        public bool HasChanged
        {
            get { return _hasChanged; }
            set
            {
                _hasChanged = value;
                NotifyPropertyChanged("HasChanged");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        private int _errorIndex = 0;
        public int ErrorIndex
        {
            get
            {
                int index = 0;

                if (!clsLanguageLibrary.IsNumeric(Stripped))
                {
                    char currentChar;

                    // er der både tal og bogstaver er den gal
                    if (clsLanguageLibrary.DigitCount(Stripped) > 0 && clsLanguageLibrary.LetterCount(Stripped) > 0)
                        index += 20;

                    // er der sjældne tegn inde i ordet, er den helt gal(punktum, bindestreg og slash går bedre an)
                    string temp = Stripped.Replace(".", "");
                    temp = temp.Replace("-", "");
                    temp = temp.Replace("/", "");

                    if (temp.Length > 1)
                    {
                        for (int i = 1; i < temp.Length; i++)       // først fra position 1, ikke 0
                        {
                            currentChar = temp[i];
                            if (char.IsPunctuation(currentChar))
                                index += 15;
                        }
                    }

                    // er der store bogstaver inde i ordet, er den gal (med mindre det er f.x. H.C.) - dvs. især, hvis det begynder med småt!
                    if (Stripped.Length > 1)
                    {
                        bool startsWithUpper = clsLanguageLibrary.StartsWithUpperCase(Stripped);

                        for (int i = 1; i < temp.Length; i++)       // først fra position 1, ikke 0
                        {
                            currentChar = temp[i];
                            if (char.IsUpper(currentChar))
                            {
                                if (startsWithUpper)
                                    index += 7;
                                else
                                    index += 22;
                            }
                        }
                    }

                    // for at kompensere for det med H.C. Andersen, tjekker vi nu flg.:
                    if (Stripped.Length > 1)
                    {
                        if (clsLanguageLibrary.UpperCaseCount(Stripped) == clsLanguageLibrary.PeriodCount(Stripped))
                            index -= 10;
                    }

                    // dog kan den så blive negativ - det er noget rod... så må vi lægge 10 til igen (ja, det er noget værre spaghettikode)
                    if (index < 0)
                        index += 10;

                    // hvordan er fordelingen af bogstaver? - og er der mange konsonanter i træk?
                    if (Stripped.Length > 2)
                    {
                        int letterCount = clsLanguageLibrary.LetterCount(Stripped);
                        if (letterCount > 0)
                        {
                            double vowelRatio = clsLanguageLibrary.VowelCount(Stripped) / letterCount;
                            if (vowelRatio > 0.75)
                                index += (int)(3.0 * vowelRatio);

                            double consonantRatio = clsLanguageLibrary.ConsonantCount(Stripped) / letterCount;
                            if (consonantRatio > 0.75)
                                index += (int)(3.0 * consonantRatio);

                            int consecutiveConsonants = clsLanguageLibrary.ConsecutiveConsonantCount(Stripped);
                            if (consecutiveConsonants > 4)
                                index += consecutiveConsonants;
                        }
                    }


                    
                    // endelig ganger vi manglende forekomster op: dvs. under 5 forekomster ganges med fra 1 til 4
                    if (OccurrenceCount < 5)
                        index = index * (5 - OccurrenceCount);
                }


                _errorIndex = index;
                return _errorIndex;
            }
        }


        // Constructor
        public clsTrLemma(clsTrWord Word)
        {
            _content = Word.Raw;
            // Stripped = Word.Stripped;

        }

        public int CompareTo(object obj)
        {
            var c = obj as clsTrLemma;
            return Content.CompareTo(c.Content);
        }

    }
}
