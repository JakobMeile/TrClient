// <copyright file="clsLanguageLibrary.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace DanishNLP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ClsLanguageLibrary
    {
        private static string monthNames = "januar;februar;marts;april;maj;juni;juli;august;september;oktober;november;december;january;february;march;may;june;july;october";
        private static List<string> monthNameList = monthNames.Split(';').ToList();

        // NB: Husk også at tilføje nye forkortelser i function nedenunder!
        private static string monthAbbreviations =
            "jan;janu;janaur;" +
            "feb;febr;febru;" +
            "mar;apr;mai;jun;jul;aug;" +
            "sep;sepb;sept;septb;septe;septr;septbr;septem;septmb;septemb;" +
            "oct;octb;octbr;okt;oktb;oktbr;oktob;" +
            "nov;nvbr;novem;novbr;novemb;novembr;novmb;" +
            "dec;dcbr;debr;decbr;decem;decemb;decembr";

        private static List<string> monthAbbreviationsList = monthAbbreviations.Split(';').ToList();

        public static string GetMonthName(int number)
        {
            string temp = string.Empty;

            switch (number)
            {
                case 1:
                    temp = "januar";
                    break;
                case 2:
                    temp = "februar";
                    break;
                case 3:
                    temp = "marts";
                    break;
                case 4:
                    temp = "april";
                    break;
                case 5:
                    temp = "maj";
                    break;
                case 6:
                    temp = "juni";
                    break;
                case 7:
                    temp = "juli";
                    break;
                case 8:
                    temp = "august";
                    break;
                case 9:
                    temp = "september";
                    break;
                case 10:
                    temp = "oktober";
                    break;
                case 11:
                    temp = "november";
                    break;
                case 12:
                    temp = "december";
                    break;
                default:
                    temp = "n/a";
                    break;
            }

            return temp;
        }

        public static int GetMonthNumber(string source)
        {
            int test = 0;
            string stripped = StripAll(source.ToLower());

            switch (stripped)
            {
                case "januar":
                case "january":
                case "jan":
                case "janu":
                case "janaur":
                    test = 1;
                    break;

                case "februar":
                case "february":
                case "feb":
                case "febr":
                case "febru":
                    test = 2;
                    break;

                case "marts":
                case "march":
                case "mar":
                    test = 3;
                    break;

                case "april":
                case "apr":
                    test = 4;
                    break;

                case "maj":
                case "mai":
                case "may":
                    test = 5;
                    break;

                case "juni":
                case "june":
                case "jun":
                    test = 6;
                    break;

                case "juli":
                case "july":
                case "jul":
                    test = 7;
                    break;

                case "august":
                case "aug":
                    test = 8;
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
                    test = 9;
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
                    test = 10;
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
                    test = 11;
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
                    test = 12;
                    break;
            }

            return test;
        }

        public static string StripAll(string source)
        {
            string temp = string.Empty;
            int firstPos = 0;
            int lastPos = source.Length - 1;
            int numberOfLetters = LetterCount(source);
            int numberOfDigits = DigitCount(source);
            int numberOfAlphaNumeric = numberOfLetters + numberOfDigits;

            if (source.Length > 0 && numberOfAlphaNumeric > 0)
            {
                while (!char.IsLetterOrDigit(source[firstPos]))
                {
                    firstPos++;
                }

                while (!char.IsLetterOrDigit(source[lastPos]))
                {
                    lastPos--;
                }

                int newLength = lastPos - firstPos + 1;
                if (newLength > 0)
                {
                    temp = source.Substring(firstPos, newLength);
                }
            }

            //if (Source != Temp)
            //    Debug.WriteLine($"StripAll: Stripping from {Source} to {Temp}");
            return temp;
        }

        public static string StripPunctuation(string source)
        {
            string punctuationMarks = " .,:;!?";
            string temp = source;

            if (temp != null)
            {
                if (temp.Length >= 2)
                {
                    string lastLetter = temp.Last().ToString();
                    int lastPos = punctuationMarks.IndexOf(lastLetter);
                    if (lastPos >= 0)
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }

                    string firstLetter = temp.First().ToString();
                    int firstPos = punctuationMarks.IndexOf(firstLetter);
                    if (firstPos >= 0)
                    {
                        temp = temp.Substring(1);
                    }
                }
            }

            //if (Source != Temp)
            //    Debug.WriteLine($"StripPunctuation: Stripping from {Source} to {Temp}");
            return temp;
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
        public static string StripQuotationMarks(string source)
        {
            string quotationMarks = "«‹»›„“‟”’‚‘‛";
            string temp = source;

            if (temp != null)
            {
                if (temp.Length >= 2)
                {
                    string lastLetter = temp.Last().ToString();
                    int lastPos = quotationMarks.IndexOf(lastLetter);
                    if (lastPos >= 0)
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }

                    string firstLetter = temp.First().ToString();
                    int firstPos = quotationMarks.IndexOf(firstLetter);
                    if (firstPos >= 0)
                    {
                        temp = temp.Substring(1);
                    }
                }
            }

            //if (Source != Temp)
            //    Debug.WriteLine($"StripQuotation: Stripping from {Source} to {Temp}");
            return temp;
        }

        public static bool EndsQuotation(string source)
        {
            string quotationMarks = "«»„“‟”";
            char currentMark;
            bool found = false;
            int midPosition = source.Length / 2;

            if (source != null)
            {
                for (int i = 0; i <= quotationMarks.Length - 1; i++)
                {
                    currentMark = quotationMarks[i];
                    found = found || source.IndexOf(currentMark) >= midPosition;
                }
            }

            return found;
        }

        public static bool StartsQuotation(string source)
        {
            string quotationMarks = "«»„“‟”";
            char currentMark;
            bool found = false;
            int midPosition = source.Length / 2;

            if (source != null)
            {
                for (int i = 0; i <= quotationMarks.Length - 1; i++)
                {
                    currentMark = quotationMarks[i];
                    found = found || (source.IndexOf(currentMark) > -1 && source.IndexOf(currentMark) <= midPosition);
                }
            }

            return found;
        }

        public static string StripParantheses(string source)
        {
            string parantheses = "()[]{}";
            string temp = source;

            if (temp != null)
            {
                if (temp.Length >= 2)
                {
                    string lastLetter = temp.Last().ToString();
                    int lastPos = parantheses.IndexOf(lastLetter);
                    if (lastPos >= 0)
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }

                    string firstLetter = temp.First().ToString();
                    int firstPos = parantheses.IndexOf(firstLetter);
                    if (firstPos >= 0)
                    {
                        temp = temp.Substring(1);
                    }
                }
            }

            //if (Source != Temp)
            //    Debug.WriteLine($"StripParantheses: Stripping from {Source} to {Temp}");
            return temp;
        }

        public static string UnHyphenate(string source)
        {
            string hyphen = "-";
            string temp = source;

            if (temp != null)
            {
                if (temp.Length >= 2)
                {
                    string lastLetter = temp.Last().ToString();
                    int lastPos = hyphen.IndexOf(lastLetter);
                    if (lastPos >= 0)
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }
                }
            }

            return temp;
        }

        public static bool EndsSentence(string source) // skal skrives noget om - så den tager hensyn til abbreviations
        {
            bool checkEnd = false;
            string endMarks = ".:!?";
            string temp = source.Trim();
            temp = StripParantheses(temp);
            temp = StripQuotationMarks(temp);

            if (temp.Length > 0)
            {
                char lastChar = temp[temp.Length - 1];
                if (endMarks.IndexOf(lastChar) != -1)
                {
                    checkEnd = true;
                }
            }

            return checkEnd;
        }

        public static bool EndsWithPeriodColonExclamationQuestionMark(string source)
        {
            bool checkEnd = false;
            string endMarks = ".:!?";
            string temp = source.Trim();
            temp = StripParantheses(temp);
            temp = StripQuotationMarks(temp);

            if (temp.Length > 0)
            {
                char lastChar = temp[temp.Length - 1];
                if (endMarks.IndexOf(lastChar) != -1)
                {
                    checkEnd = true;
                }
            }

            return checkEnd;
        }

        public static bool EndsWithComma(string source)
        {
            bool checkEnd = false;
            string endMarks = ",";
            string temp = source.Trim();
            temp = StripParantheses(temp);
            temp = StripQuotationMarks(temp);

            if (temp.Length > 0)
            {
                char lastChar = temp[temp.Length - 1];
                if (endMarks.IndexOf(lastChar) != -1)
                {
                    checkEnd = true;
                }
            }

            // Debug.WriteLine($"{Source} - ends with comma? {CheckEnd}");
            return checkEnd;
        }

        public static bool EndsWithHyphen(string source)
        {
            bool checkHyphen = false;

            if (source.Length > 2)
            {
                char lastChar = source[source.Length - 1];
                if (lastChar == '-')
                {
                    checkHyphen = true;
                }
            }

            return checkHyphen;
        }

        public static bool EndsWithAlphaNumeric(string source)
        {
            bool checkAlphaNumeric = false;

            if (source.Length > 2)
            {
                char lastChar = source[source.Length - 1];
                if (char.IsLetterOrDigit(lastChar))
                {
                    checkAlphaNumeric = true;
                }
            }

            return checkAlphaNumeric;
        }

        public static bool StartsWithUpperCase(string source)
        {
            string temp = source.Trim();
            temp = StripParantheses(temp);
            temp = StripQuotationMarks(temp);

            bool checkFirst;

            if (temp.Length > 0)
            {
                char firstChar = temp[0];
                checkFirst = char.IsUpper(firstChar);
            }
            else
            {
                checkFirst = false;
            }

            return checkFirst;
        }

        public static bool StartsWithLowerCase(string source)
        {
            string temp = source.Trim();
            temp = StripParantheses(temp);
            temp = StripQuotationMarks(temp);

            bool checkFirst;

            if (temp.Length > 0)
            {
                char firstChar = temp[0];
                checkFirst = char.IsLower(firstChar);
            }
            else
            {
                checkFirst = false;
            }

            return checkFirst;
        }

        public static bool StartsWithVowel(string source)
        {
            string temp = source.Trim();
            temp = StripParantheses(temp);
            temp = StripQuotationMarks(temp);

            bool checkFirst;

            if (temp.Length > 0)
            {
                char firstChar = temp[0];
                checkFirst = IsVowel(firstChar);
            }
            else
            {
                checkFirst = false;
            }

            return checkFirst;
        }

        public static bool StartsWithConsonant(string source)
        {
            string temp = source.Trim();
            temp = StripParantheses(temp);
            temp = StripQuotationMarks(temp);

            bool checkFirst;

            if (temp.Length > 0)
            {
                char firstChar = temp[0];
                checkFirst = IsConsonant(firstChar);
            }
            else
            {
                checkFirst = false;
            }

            return checkFirst;
        }

        public static bool IsAllUpperCase(string source)
        {
            // NB: whitespace og punctuation tæller med som UPPER!!
            bool checkUpper = true;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    checkUpper = checkUpper && (char.IsUpper(currentChar) || char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar));
                }
            }
            else
            {
                checkUpper = false;
            }

            return checkUpper;
        }

        public static bool IsAllLowerCase(string source)
        {
            // NB: whitespace og punctuation tæller med som LOWER!!
            bool checkLower = true;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    checkLower = checkLower && (char.IsLower(currentChar) || char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar));
                }
            }
            else
            {
                checkLower = false;
            }

            return checkLower;
        }

        public static bool IsMonthName(string source)
        {
            return monthNameList.Contains(source.ToLower());
        }

        public static bool IsMonthAbbreviation(string source)
        {
            return monthAbbreviationsList.Contains(source.ToLower());
        }

        public static bool IsPossibleDate(string source)
        {
            bool checkChar = true;

            if (source.Length > 0)
            {
                // tjekker først, om strengen består af cifre, whitespace og punktuering
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    checkChar = checkChar && (char.IsNumber(currentChar) || char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar) || currentChar == '/');
                }

                // tjekker dernæst, om det er en dato af formen nn. måned yyyy - alternativt blot måned yyyy
                if (!checkChar)
                {
                    checkChar = true;
                    List<string> sourceList = source.Split(' ').ToList();

                    foreach (string s in sourceList)
                    {
                        bool test = monthNameList.Contains(s.ToLower()) || monthAbbreviationsList.Contains(s.ToLower()) || IsNumeric(s);
                        checkChar = checkChar && test;
                    }
                }
            }
            else
            {
                checkChar = false;
            }

            return checkChar;
        }

        public static bool IsPossiblePlaceName(string source)
        {
            bool check = true;

            return check;
        }

        public static bool IsPossibleAddress(string source)
        {
            bool check = true;

            return check;
        }

        public static bool IsNumeric(object expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static bool IsOrdinalNumber(string source)
        {
            bool oK = true;

            // først fjernes paranteser, som ofte anvendes ved senere kongenavne, f.eks. Christian (10.)
            string temp = ClsLanguageLibrary.StripParantheses(source);

            if (temp.Length > 1)
            {
                int periodPos = temp.IndexOf('.');
                if (periodPos != -1)
                {
                    char currentChar;
                    for (int i = 0; i < periodPos; i++)
                    {
                        currentChar = temp[i];
                        oK = oK && char.IsNumber(currentChar);
                    }
                }
                else
                {
                    oK = false;
                }
            }
            else
            {
                oK = false;
            }

            return oK;
        }

        public static bool IsVowel(char c)
        {
            string vowels = "aeiouyæøåáéíóúàèìòùäëïöüÿâêîôû";
            int position = vowels.IndexOf(c.ToString().ToLower());
            bool result = position > -1;
            return result;
        }

        public static bool IsAllVowel(string source)
        {
            bool result = LetterCount(source) == VowelCount(source);
            return result;
        }

        public static bool IsConsonant(char c)
        {
            string consonants = "bcdfghjklmnpqrstvwxz";
            int position = consonants.IndexOf(c.ToString().ToLower());
            bool result = position > -1;
            return result;
        }

        public static bool IsAllConsonant(string source)
        {
            bool result = LetterCount(source) == ConsonantCount(source);
            return result;
        }

        public static int LetterCount(string source)
        {
            int count = 0;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (char.IsLetter(currentChar))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int DigitCount(string source)
        {
            int count = 0;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (char.IsDigit(currentChar))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int PunctuationCount(string source)
        {
            int count = 0;
            string temp = source.Replace("-", string.Empty);

            // ja, hyphen fjernes, for ellers slår fx sammensatte adelsnavne ud som forkortelser :)

            // bool HasHyphen = Source.IndexOf("-") >= 0;
            if (temp.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < temp.Length; i++)
                {
                    currentChar = temp[i];
                    if (char.IsPunctuation(currentChar))
                    {
                        count++;
                    }
                }
            }

            //if (Count > 0 && HasHyphen)
            //    Count--;
            return count;
        }

        public static int PeriodCount(string source)
        {
            int count = 0;
            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (currentChar == '.')
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int UpperCaseCount(string source)
        {
            int count = 0;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (char.IsUpper(currentChar))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int LowerCaseCount(string source)
        {
            int count = 0;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (char.IsLower(currentChar))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int VowelCount(string source)
        {
            int count = 0;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (IsVowel(currentChar))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int ConsonantCount(string source)
        {
            int count = 0;

            if (source.Length > 0)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (IsConsonant(currentChar))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int ConsecutiveConsonantCount(string source)
        {
            int count = 0;
            int max = 0;

            if (source.Length > 3)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (IsConsonant(currentChar))
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    if (count > max)
                    {
                        max = count;
                    }
                }
            }

            return max;
        }

        public static int ConsecutiveVowelCount(string source)
        {
            int count = 0;
            int max = 0;

            if (source.Length > 3)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (IsVowel(currentChar))
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    if (count > max)
                    {
                        max = count;
                    }
                }
            }

            return max;
        }

        public static int ConsecutiveDigitCount(string source)
        {
            int count = 0;
            int max = 0;

            if (source.Length > 3)
            {
                char currentChar;
                for (int i = 0; i < source.Length; i++)
                {
                    currentChar = source[i];
                    if (IsNumeric(currentChar))
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    if (count > max)
                    {
                        max = count;
                    }
                }
            }

            return max;
        }
    }
}
