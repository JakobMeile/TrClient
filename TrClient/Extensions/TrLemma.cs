// <copyright file="TrLemma.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DanishNLP;

    public class TrLemma : IComparable, INotifyPropertyChanged
    {
        public TrLemmas ParentContainer;

        public List<TrWord> Occurrences = new List<TrWord>();

        private string content;

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                if (content != value)
                {
                    content = value;

                    foreach (TrWord word in Occurrences)
                    {
                        word.Raw = content;
                    }

                    NotifyPropertyChanged("Content");
                    NotifyPropertyChanged("Stripped");
                    NotifyPropertyChanged("ErrorIndex");
                    HasChanged = true;
                }
            }
        }

        private string stripped;

        public string Stripped
        {
            get
            {
                if (Content == "&" || Content == "…" || Content == "[…]" || Content == "(…)" || IsOrdinalNumber)
                {
                    stripped = Content;
                }
                else
                {
                    stripped = ClsLanguageLibrary.StripAll(Content);
                }

                return stripped;
            }
        }

        private bool isOrdinalNumber;

        public bool IsOrdinalNumber
        {
            get
            {
                // her bruges RAW, fordi det afsluttende punktum er centralt!
                isOrdinalNumber = ClsLanguageLibrary.IsOrdinalNumber(Content);
                return isOrdinalNumber;
            }
        }

        public bool MarkToDeletion = false;

        private int occurrenceCount;

        public int OccurrenceCount
        {
            get
            {
                occurrenceCount = Occurrences.Count;
                return occurrenceCount;
            }
        }

        private bool hasChanged = false;

        public bool HasChanged
        {
            get
            {
                return hasChanged;
            }

            set
            {
                hasChanged = value;
                NotifyPropertyChanged("HasChanged");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private int errorIndex = 0;

        public int ErrorIndex
        {
            get
            {
                int index = 0;

                if (!ClsLanguageLibrary.IsNumeric(Stripped))
                {
                    char currentChar;

                    // er der både tal og bogstaver er den gal
                    if (ClsLanguageLibrary.DigitCount(Stripped) > 0 && ClsLanguageLibrary.LetterCount(Stripped) > 0)
                    {
                        index += 20;
                    }

                    // er der sjældne tegn inde i ordet, er den helt gal(punktum, bindestreg og slash går bedre an)
                    string temp = Stripped.Replace(".", string.Empty);
                    temp = temp.Replace("-", string.Empty);
                    temp = temp.Replace("/", string.Empty);

                    if (temp.Length > 1)
                    {
                        for (int i = 1; i < temp.Length; i++)       // først fra position 1, ikke 0
                        {
                            currentChar = temp[i];
                            if (char.IsPunctuation(currentChar))
                            {
                                index += 15;
                            }
                        }
                    }

                    // er der store bogstaver inde i ordet, er den gal (med mindre det er f.x. H.C.) - dvs. især, hvis det begynder med småt!
                    if (Stripped.Length > 1)
                    {
                        bool startsWithUpper = ClsLanguageLibrary.StartsWithUpperCase(Stripped);

                        for (int i = 1; i < temp.Length; i++)       // først fra position 1, ikke 0
                        {
                            currentChar = temp[i];
                            if (char.IsUpper(currentChar))
                            {
                                if (startsWithUpper)
                                {
                                    index += 7;
                                }
                                else
                                {
                                    index += 22;
                                }
                            }
                        }
                    }

                    // for at kompensere for det med H.C. Andersen, tjekker vi nu flg.:
                    if (Stripped.Length > 1)
                    {
                        if (ClsLanguageLibrary.UpperCaseCount(Stripped) == ClsLanguageLibrary.PeriodCount(Stripped))
                        {
                            index -= 10;
                        }
                    }

                    // dog kan den så blive negativ - det er noget rod... så må vi lægge 10 til igen (ja, det er noget værre spaghettikode)
                    if (index < 0)
                    {
                        index += 10;
                    }

                    // hvordan er fordelingen af bogstaver? - og er der mange konsonanter i træk?
                    if (Stripped.Length > 2)
                    {
                        int letterCount = ClsLanguageLibrary.LetterCount(Stripped);
                        if (letterCount > 0)
                        {
                            double vowelRatio = ClsLanguageLibrary.VowelCount(Stripped) / letterCount;
                            if (vowelRatio > 0.75)
                            {
                                index += (int)(3.0 * vowelRatio);
                            }

                            double consonantRatio = ClsLanguageLibrary.ConsonantCount(Stripped) / letterCount;
                            if (consonantRatio > 0.75)
                            {
                                index += (int)(3.0 * consonantRatio);
                            }

                            int consecutiveConsonants = ClsLanguageLibrary.ConsecutiveConsonantCount(Stripped);
                            if (consecutiveConsonants > 4)
                            {
                                index += consecutiveConsonants;
                            }
                        }
                    }

                    // endelig ganger vi manglende forekomster op: dvs. under 5 forekomster ganges med fra 1 til 4
                    if (OccurrenceCount < 5)
                    {
                        index = index * (5 - OccurrenceCount);
                    }
                }

                errorIndex = index;
                return errorIndex;
            }
        }

        // Constructor
        public TrLemma(TrWord word)
        {
            content = word.Raw;

            // Stripped = Word.Stripped;
        }

        public int CompareTo(object obj)
        {
            var c = obj as TrLemma;
            return Content.CompareTo(c.Content);
        }
    }
}
