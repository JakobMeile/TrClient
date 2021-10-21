using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DanishNLP;


namespace DanishNLP
{
    public static class clsLanguageLibrary
    {
        private static string MonthNames = "januar;februar;marts;april;maj;juni;juli;august;september;oktober;november;december;january;february;march;may;june;july;october";
        private static List<string> MonthNameList = MonthNames.Split(';').ToList();

        // NB: Husk også at tilføje nye forkortelser i function nedenunder!
        private static string MonthAbbreviations = 
            "jan;janu;janaur;" +
            "feb;febr;febru;" +
            "mar;apr;mai;jun;jul;aug;" +
            "sep;sepb;sept;septb;septe;septr;septbr;septem;septmb;septemb;" +
            "oct;octb;octbr;okt;oktb;oktbr;oktob;" +
            "nov;nvbr;novem;novbr;novemb;novembr;novmb;" +
            "dec;dcbr;debr;decbr;decem;decemb;decembr";

        private static List<string> MonthAbbreviationsList = MonthAbbreviations.Split(';').ToList();

        public static string GetMonthName(int Number)
        {
            string Temp = "";

            switch (Number)
            {
                case 1:
                    Temp = "januar";
                    break;
                case 2:
                    Temp = "februar";
                    break;
                case 3:
                    Temp = "marts";
                    break;
                case 4:
                    Temp = "april";
                    break;
                case 5:
                    Temp = "maj";
                    break;
                case 6:
                    Temp = "juni";
                    break;
                case 7:
                    Temp = "juli";
                    break;
                case 8:
                    Temp = "august";
                    break;
                case 9:
                    Temp = "september";
                    break;
                case 10:
                    Temp = "oktober";
                    break;
                case 11:
                    Temp = "november";
                    break;
                case 12:
                    Temp = "december";
                    break;
                default:
                    Temp = "n/a";
                    break;
            }
            return Temp;
        }

        public static int GetMonthNumber(string Source)
        {
            int Test = 0;
            string Stripped = StripAll(Source.ToLower());

            switch (Stripped)
            {
                case "januar":
                case "january":
                case "jan":
                case "janu":
                case "janaur":
                    Test = 1;
                    break;

                case "februar":
                case "february":
                case "feb":
                case "febr":
                case "febru":
                    Test = 2;
                    break;

                case "marts":
                case "march":
                case "mar":
                    Test = 3;
                    break;

                case "april":
                case "apr":
                    Test = 4;
                    break;

                case "maj":
                case "mai":
                case "may":
                    Test = 5;
                    break;

                case "juni":
                case "june":
                case "jun":
                    Test = 6;
                    break;

                case "juli":
                case "july":
                case "jul":
                    Test = 7;
                    break;

                case "august":
                case "aug":
                    Test = 8;
                    break;

                // "sep;sepb;sept;septb;septe;septr;septbr;septem;septmb;septemb;" +

                case "september":
                case "sep":
                case "sepb":
                case "sept":
                case "septb":
                case "septe":
                case "septr":
                case "septbr":
                case "septem":
                case "septmb":
                case "septemb":
                    Test = 9;
                    break;

                //  "oct;octb;octbr;okt;oktb;oktbr;oktob;" +

                case "oktober":
                case "october":
                case "oct":
                case "octb":
                case "octbr":
                case "okt":
                case "oktb":
                case "oktbr":
                case "oktob":
                    Test = 10;
                    break;

                //  "nov;nvbr;novem;novbr;novemb;novembr;novmb;" +

                case "november":
                case "nov":
                case "nvbr":
                case "novbr":
                case "novem":
                case "novmb":
                case "novemb":
                case "novembr":
                    Test = 11;
                    break;

                //  "dec;dcbr;debr;decbr;decem;decemb;decembr";

                case "december":
                case "dec":
                case "dcbr":
                case "debr":
                case "decbr":
                case "decem":
                case "decemb":
                case "decembr":
                    Test = 12;
                    break;
            }
            return Test;
        }


        public static string StripAll(string Source)
        {
            string Temp = "";
            int FirstPos = 0;
            int LastPos = Source.Length - 1;
            int NumberOfLetters = LetterCount(Source);
            int NumberOfDigits = DigitCount(Source);
            int NumberOfAlphaNumeric = NumberOfLetters + NumberOfDigits;
            
            if (Source.Length > 0 && NumberOfAlphaNumeric > 0)
            {
                while (!char.IsLetterOrDigit(Source[FirstPos]))
                    FirstPos++;

                while (!char.IsLetterOrDigit(Source[LastPos]))
                    LastPos--;

                int NewLength = LastPos - FirstPos + 1;
                if (NewLength > 0)
                    Temp = Source.Substring(FirstPos, NewLength);
            }
            //if (Source != Temp)
            //    Debug.WriteLine($"StripAll: Stripping from {Source} to {Temp}");
            return Temp;
        }

        public static string StripPunctuation(string Source)
        {
            string PunctuationMarks = " .,:;!?";
            string Temp = Source;

            if (Temp != null)
                if (Temp.Length >= 2)
                {
                    string LastLetter = Temp.Last().ToString();
                    int LastPos = PunctuationMarks.IndexOf(LastLetter);
                    if (LastPos >= 0)
                        Temp = Temp.Substring(0, Temp.Length - 1);

                    string FirstLetter = Temp.First().ToString();
                    int FirstPos = PunctuationMarks.IndexOf(FirstLetter);
                    if (FirstPos >= 0)
                        Temp = Temp.Substring(1);
                }
            //if (Source != Temp)
            //    Debug.WriteLine($"StripPunctuation: Stripping from {Source} to {Temp}");
            return Temp;
        }

        //public static string StripSpecificEndingChars(string Source, string CharsToStrip)
        //{
        //    string Temp = Source;
        //    int LastPos = Source.Length - 1;

        //    if (Source.Length > 0)
        //    {
        //        while (CharsToStrip.IndexOf(Source[LastPos]) >= 0)
        //        {
        //            Debug.WriteLine($"Lastpos = {LastPos}, Char at Lastpos = {Source[LastPos]}");
        //            LastPos--;

        //        }

        //        Temp = Source.Substring(0, LastPos);
        //    }

        //    return Temp;
        //}


        public static string StripQuotationMarks(string Source)
        {
            string QuotationMarks = "«‹»›„“‟”’‚‘‛";
            string Temp = Source;

            if (Temp != null)
                if (Temp.Length >= 2)
                {
                    string LastLetter = Temp.Last().ToString();
                    int LastPos = QuotationMarks.IndexOf(LastLetter);
                    if (LastPos >= 0)
                        Temp = Temp.Substring(0, Temp.Length - 1);

                    string FirstLetter = Temp.First().ToString();
                    int FirstPos = QuotationMarks.IndexOf(FirstLetter);
                    if (FirstPos >= 0)
                        Temp = Temp.Substring(1);
                }
            //if (Source != Temp)
            //    Debug.WriteLine($"StripQuotation: Stripping from {Source} to {Temp}");
            return Temp;
        }

        public static bool EndsQuotation(string Source)
        {
            string QuotationMarks = "«»„“‟”";
            char CurrentMark;
            bool Found = false;
            int MidPosition = Source.Length / 2;

            if (Source != null)
            {
                for (int i = 0; i <= QuotationMarks.Length - 1; i++)
                {
                    CurrentMark = QuotationMarks[i];
                    Found = Found || Source.IndexOf(CurrentMark) >= MidPosition;
                }
            }
            return Found;
        }

        public static bool StartsQuotation(string Source)
        {
            string QuotationMarks = "«»„“‟”";
            char CurrentMark;
            bool Found = false;
            int MidPosition = Source.Length / 2;

            if (Source != null)
            {
                for (int i = 0; i <= QuotationMarks.Length - 1; i++)
                {
                    CurrentMark = QuotationMarks[i];
                    Found = Found || (Source.IndexOf(CurrentMark) > -1 && Source.IndexOf(CurrentMark) <= MidPosition);
                }
            }
            return Found;
        }


        public static string StripParantheses(string Source)
        {
            string Parantheses = "()[]{}";
            string Temp = Source;

            if (Temp != null)
                if (Temp.Length >= 2)
                {
                    string LastLetter = Temp.Last().ToString();
                    int LastPos = Parantheses.IndexOf(LastLetter);
                    if (LastPos >= 0)
                        Temp = Temp.Substring(0, Temp.Length - 1);

                    string FirstLetter = Temp.First().ToString();
                    int FirstPos = Parantheses.IndexOf(FirstLetter);
                    if (FirstPos >= 0)
                        Temp = Temp.Substring(1);
                }
            //if (Source != Temp)
            //    Debug.WriteLine($"StripParantheses: Stripping from {Source} to {Temp}");
            return Temp;
        }

        

        public static string UnHyphenate(string Source)
        {
            string Hyphen = "-";
            string Temp = Source;

            if (Temp != null)
                if (Temp.Length >= 2)
                {
                    string LastLetter = Temp.Last().ToString();
                    int LastPos = Hyphen.IndexOf(LastLetter);
                    if (LastPos >= 0)
                        Temp = Temp.Substring(0, Temp.Length - 1);
                }
            return Temp;

        }

        public static bool EndsSentence(string Source) // skal skrives noget om - så den tager hensyn til abbreviations
        {
            bool CheckEnd = false;
            string EndMarks = ".:!?";
            string Temp = Source.Trim();
            Temp = StripParantheses(Temp);
            Temp = StripQuotationMarks(Temp);

            if (Temp.Length > 0)
            {
                char LastChar = Temp[Temp.Length - 1];
                if (EndMarks.IndexOf(LastChar) != -1)
                    CheckEnd = true;
            }
            return CheckEnd;
        }

        public static bool EndsWithPeriodColonExclamationQuestionMark(string Source)
        {
            bool CheckEnd = false;
            string EndMarks = ".:!?";
            string Temp = Source.Trim();
            Temp = StripParantheses(Temp);
            Temp = StripQuotationMarks(Temp);

            if (Temp.Length > 0)
            {
                char LastChar = Temp[Temp.Length - 1];
                if (EndMarks.IndexOf(LastChar) != -1)
                    CheckEnd = true;
            }
            return CheckEnd;
        }

        
        public static bool EndsWithComma(string Source)
        {
            bool CheckEnd = false;
            string EndMarks = ",";
            string Temp = Source.Trim();
            Temp = StripParantheses(Temp);
            Temp = StripQuotationMarks(Temp);

            if (Temp.Length > 0)
            {
                char LastChar = Temp[Temp.Length - 1];
                if (EndMarks.IndexOf(LastChar) != -1)
                    CheckEnd = true;
            }
            // Debug.WriteLine($"{Source} - ends with comma? {CheckEnd}");
            return CheckEnd;
        }


        public static bool EndsWithHyphen(string Source)
        {
            bool CheckHyphen = false;

            if (Source.Length > 2)
            {
                char LastChar = Source[Source.Length - 1];
                if (LastChar == '-')
                    CheckHyphen = true;
            }
            return CheckHyphen;
        }

        public static bool EndsWithAlphaNumeric(string Source)
        {
            bool CheckAlphaNumeric = false;

            if (Source.Length > 2)
            {
                char LastChar = Source[Source.Length - 1];
                if (char.IsLetterOrDigit(LastChar))
                    CheckAlphaNumeric = true;
            }
            return CheckAlphaNumeric;
        }


        public static bool StartsWithUpperCase(string Source)
        {
            string Temp = Source.Trim();
            Temp = StripParantheses(Temp);
            Temp = StripQuotationMarks(Temp);

            bool CheckFirst;

            if (Temp.Length > 0)
            {
                char FirstChar = Temp[0];
                CheckFirst = char.IsUpper(FirstChar);
            }
            else
                CheckFirst = false;

            return CheckFirst;
        }
        
        public static bool StartsWithLowerCase(string Source)
        {
            string Temp = Source.Trim();
            Temp = StripParantheses(Temp);
            Temp = StripQuotationMarks(Temp);

            bool CheckFirst;

            if (Temp.Length > 0)
            {
                char FirstChar = Temp[0];
                CheckFirst = char.IsLower(FirstChar);
            }
            else
                CheckFirst = false;

            return CheckFirst;
        }

        public static bool StartsWithVowel(string Source)
        {
            string Temp = Source.Trim();
            Temp = StripParantheses(Temp);
            Temp = StripQuotationMarks(Temp);

            bool CheckFirst;

            if (Temp.Length > 0)
            {
                char FirstChar = Temp[0];
                CheckFirst = IsVowel(FirstChar);
            }
            else
                CheckFirst = false;

            return CheckFirst;
        }

        public static bool StartsWithConsonant(string Source)
        {
            string Temp = Source.Trim();
            Temp = StripParantheses(Temp);
            Temp = StripQuotationMarks(Temp);

            bool CheckFirst;

            if (Temp.Length > 0)
            {
                char FirstChar = Temp[0];
                CheckFirst = IsConsonant(FirstChar);
            }
            else
                CheckFirst = false;

            return CheckFirst;
        }


        public static bool IsAllUpperCase(string Source)
        {
            // NB: whitespace og punctuation tæller med som UPPER!!

            bool CheckUpper = true;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    CheckUpper = CheckUpper && (char.IsUpper(CurrentChar) || char.IsWhiteSpace(CurrentChar) || char.IsPunctuation(CurrentChar));
                }
            }
            else
                CheckUpper = false;

            return CheckUpper;
        }
        
        public static bool IsAllLowerCase(string Source)
        {
            // NB: whitespace og punctuation tæller med som LOWER!!
            bool CheckLower = true;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    CheckLower = CheckLower && (char.IsLower(CurrentChar) || char.IsWhiteSpace(CurrentChar) || char.IsPunctuation(CurrentChar));
                }
            }
            else
                CheckLower = false;

            return CheckLower;
        }

        public static bool IsMonthName(string Source)
        {
            return MonthNameList.Contains(Source.ToLower());
        }

        public static bool IsMonthAbbreviation(string Source)
        {
            return MonthAbbreviationsList.Contains(Source.ToLower());
        }


        public static bool IsPossibleDate(string Source)
        {
            bool CheckChar = true;

            if (Source.Length > 0)
            {
                // tjekker først, om strengen består af cifre, whitespace og punktuering            
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    CheckChar = CheckChar && (char.IsNumber(CurrentChar) || char.IsWhiteSpace(CurrentChar) || char.IsPunctuation(CurrentChar) || CurrentChar == '/');
                }
                
                // tjekker dernæst, om det er en dato af formen nn. måned yyyy - alternativt blot måned yyyy
                if (!CheckChar)
                {
                    CheckChar = true;
                    List<string> SourceList = Source.Split(' ').ToList();
                    
                    foreach (string S in SourceList)
                    {
                        bool Test = (MonthNameList.Contains(S.ToLower()) || MonthAbbreviationsList.Contains(S.ToLower()) || IsNumeric(S));
                        CheckChar = CheckChar && Test;
                    }
                }
            }
            else
                CheckChar = false;

            return CheckChar;
        }

        public static bool IsPossiblePlaceName(string Source)
        {
            bool Check = true;



            return Check;
        }

        public static bool IsPossibleAddress(string Source)
        {
            bool Check = true;



            return Check;
        }

        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static bool IsOrdinalNumber(string Source)
        {
            bool OK = true;
            // først fjernes paranteser, som ofte anvendes ved senere kongenavne, f.eks. Christian (10.)
            string Temp = clsLanguageLibrary.StripParantheses(Source);

            if (Temp.Length > 1)
            {
                int PeriodPos = Temp.IndexOf('.');
                if (PeriodPos != -1)
                {
                    char CurrentChar;
                    for (int i = 0; i < PeriodPos; i++)
                    {
                        CurrentChar = Temp[i];
                        OK = OK && char.IsNumber(CurrentChar);
                    }
                }
                else
                    OK = false;
            }
            else
                OK = false;
            return OK;
        }


        public static bool IsVowel(char C)
        {
            string Vowels = "aeiouyæøåáéíóúàèìòùäëïöüÿâêîôû";
            int Position = Vowels.IndexOf(C.ToString().ToLower());
            bool Result = Position > -1;
            return Result;
        }

        public static bool IsAllVowel(string Source)
        {
            bool Result = LetterCount(Source) == VowelCount(Source);
            return Result;
        }

        public static bool IsConsonant(char C)
        {
            string Consonants = "bcdfghjklmnpqrstvwxz";
            int Position = Consonants.IndexOf(C.ToString().ToLower());
            bool Result = Position > -1;
            return Result;
        }

        public static bool IsAllConsonant(string Source)
        {
            bool Result = LetterCount(Source) == ConsonantCount(Source);
            return Result;
        }

        public static int LetterCount(string Source)
        {
            int Count = 0;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (char.IsLetter(CurrentChar))
                        Count++;
                }
            }
            return Count;
        }

        public static int DigitCount(string Source)
        {
            int Count = 0;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (char.IsDigit(CurrentChar))
                        Count++;
                }
            }
            return Count;
        }

        public static int PunctuationCount(string Source)
        {
            int Count = 0;
            string Temp = Source.Replace("-", "");
            // ja, hyphen fjernes, for ellers slår fx sammensatte adelsnavne ud som forkortelser :)

            // bool HasHyphen = Source.IndexOf("-") >= 0;

            if (Temp.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Temp.Length; i++)
                {
                    CurrentChar = Temp[i];
                    if (char.IsPunctuation(CurrentChar))
                        Count++;
                }
            }
            //if (Count > 0 && HasHyphen)
            //    Count--;

            return Count;
        }

        public static int PeriodCount(string Source)
        {
            int Count = 0;
            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (CurrentChar == '.')
                        Count++;
                }
            }

            return Count;
        }

        public static int UpperCaseCount(string Source)
        {
            int Count = 0;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (char.IsUpper(CurrentChar))
                        Count++;
                }
            }
            return Count;
        }

        public static int LowerCaseCount(string Source)
        {
            int Count = 0;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (char.IsLower(CurrentChar))
                        Count++;
                }
            }
            return Count;
        }

        public static int VowelCount(string Source)
        {
            int Count = 0;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (IsVowel(CurrentChar))
                        Count++;
                }
            }
            return Count;
        }

        public static int ConsonantCount(string Source)
        {
            int Count = 0;

            if (Source.Length > 0)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (IsConsonant(CurrentChar))
                        Count++;
                }
            }
            return Count;
        }

        public static int ConsecutiveConsonantCount(string Source)
        {
            int Count = 0;
            int Max = 0;

            if (Source.Length > 3)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (IsConsonant(CurrentChar))
                        Count++;
                    else
                        Count = 0;

                    if (Count > Max)
                        Max = Count;
                }
            }
            return Max;
        }

        public static int ConsecutiveVowelCount(string Source)
        {
            int Count = 0;
            int Max = 0;

            if (Source.Length > 3)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (IsVowel(CurrentChar))
                        Count++;
                    else
                        Count = 0;

                    if (Count > Max)
                        Max = Count;
                }
            }
            return Max;
        }

        public static int ConsecutiveDigitCount(string Source)
        {
            int Count = 0;
            int Max = 0;

            if (Source.Length > 3)
            {
                char CurrentChar;
                for (int i = 0; i < Source.Length; i++)
                {
                    CurrentChar = Source[i];
                    if (IsNumeric(CurrentChar))
                        Count++;
                    else
                        Count = 0;

                    if (Count > Max)
                        Max = Count;
                }
            }
            return Max;
        }

    }

}
