// <copyright file="TrLibrary.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using TrClient.Core;

    // funktioner til understøttelse af TrClient
    public static class TrLibrary
    {
        public enum MarcFelt
        {
            Udefineret,
            UdgivelsesAar,      // 008a
            Opstilling,         // 096a
            Forfatter,          // 100a+h
            Titel,              // 245a+c+e+n+p
            Udgivelse,          // 260a+b+c
            Datering,           // 260c
            Beskrivelse,        // 300a
            Indhold,            // 530a
            Indbinding,         // 563a
            TitelSomEmneord,    // 645a+b
            Ophav,              // 700a+h
            BrevAfsender,       // 700a+h+4
            BrevModtager,        // 700a+h+4
        }

        // SETTINGS
        public static bool OfflineMode = false;
        public static bool LoadOnlyNewestTranscript = true;
        public static bool KOBACC = true;  // skal sættes

        public static char CSVDelimiter = '\t';
        public static char NullChar = '\0';

        //public static string OfflineBaseFolder = @"C:\Users\jakob\Dropbox\KB\Transkribus\SYNC\";

        //public static string ExportFolder = @"C:\Users\jakob\Dropbox\KB\Transkribus\TrClientGUI_Export\";
        //public static string LogFolder = @"C:\Users\jakob\Dropbox\KB\Transkribus\TrClientGUI_Log\";
        public static XNamespace Xmlns = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15";
        public static XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public static XNamespace SchemaLocation = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15 http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15/pagecontent.xsd";

        public static string TrpBaseAdress = "https://transkribus.eu/TrpServer/rest/";
        public static string TrpLogin = "https://transkribus.eu/TrpServer/rest/auth/login";
        public static string TrpCollections = "https://transkribus.eu/TrpServer/rest/collections/list.xml";

        public static string AppName = "Transkribus Client";

        public static int NarrowColumnWidth = 8;
        public static int BroadColumnWidth = 4 * NarrowColumnWidth;

        public static string EscapeString(string source)
        {
            string temp = source;

            // kolon
            temp = temp.Replace(":", "\\u003A");

            // semikolon
            temp = temp.Replace(":", "\\u003B");

            // højre tuborg
            temp = temp.Replace(":", "\\u007D");

            return temp;
        }

        public static long GetNewTimeStamp()
        {
            long elapsed = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return elapsed;
        }

        public static DateTime ConvertUnixTimeStamp(string timestamp)
        {
            DateTimeOffset dtOffset = new DateTimeOffset();
            dtOffset = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(timestamp));
            DateTime date = dtOffset.DateTime;
            return date;
        }

        public static DateTime ConvertFromISO_Time(string dateString)
        {
            DateTime newDate = DateTime.ParseExact(dateString, "yyyy-MM-ddThh:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            return newDate;
        }

        public static string ConvertToISO_Time(DateTime dateTimeObject)
        {
            return dateTimeObject.ToString("yyyy-MM-ddThh:mm:ss.fffzzz");
        }

        public static bool IsNumeric(object expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static bool IsValidDate(int year, int month, int day)
        {
            return year >= 1 && year <= 9999
                    && month >= 1 && month <= 12
                    && day >= 1 && day <= DateTime.DaysInMonth(year, month);
        }

        public static int GetReadingOrder(string attribute)
        {
            // i regioner fx. custom="structure {type:_NoTag; } readingOrder {index:0; }"
            string temp = attribute;
            if (temp.Contains("readingOrder"))
            {
                int position = temp.IndexOf("readingOrder");
                temp = temp.Substring(position, temp.Length - position);
                temp = temp.Replace("readingOrder {index:", string.Empty);
                temp = temp.Remove(temp.IndexOf(";"));
                int tempInt = (int)Convert.ToInt32(temp);
                return tempInt + 1;
            }
            else
            {
                return 0;
            }
        }

        public static int GetAverageYcoord(string coords)
        {
            int tempResult = -1;
            if (coords != string.Empty)
            {
                string temp = coords.Replace(" ", ";");
                var pointsArray = temp.Split(';').ToArray();
                int pointsCount = pointsArray.Length;

                int sumYcoord = 0;

                for (int i = 0; i < pointsCount; i++)
                {
                    int commaPos = pointsArray[i].IndexOf(",");
                    int y = (int)Convert.ToInt32(pointsArray[i].Substring(commaPos + 1));

                    sumYcoord += y;
                }

                tempResult = (int)(sumYcoord / pointsCount);
            }

            return tempResult;
        }

        public static int GetLeftMostXcoord(string coords)
        {
            int tempResult = -1;
            if (coords != string.Empty)
            {
                string temp = coords.Replace(" ", ";");
                var pointsArray = temp.Split(';').ToArray();
                int pointsCount = pointsArray.Length;

                int minX = 10000;

                for (int i = 0; i < pointsCount; i++)
                {
                    int commaPos = pointsArray[i].IndexOf(",");
                    int x = (int)Convert.ToInt32(pointsArray[i].Substring(0, commaPos));
                    if (x < minX)
                    {
                        minX = x;
                    }
                }

                tempResult = minX;
            }

            return tempResult;
        }

        public static int GetRightMostXcoord(string coords)
        {
            string temp = coords.Replace(" ", ";");
            var pointsArray = temp.Split(';').ToArray();
            int pointsCount = pointsArray.Length;

            int maxX = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                int commaPos = pointsArray[i].IndexOf(",");
                if (commaPos > -1)
                {
                    int x = (int)Convert.ToInt32(pointsArray[i].Substring(0, commaPos));
                    if (x > maxX)
                    {
                        maxX = x;
                    }
                }
            }

            return maxX;
        }

        public static bool CheckBaseLineStraightness(string coordsString, double maxAllowedAngle)
        {
            TrCoords coords = new TrCoords(coordsString);
            int highestIndex = coords.Count - 1;
            int deltaX = 0;
            int deltaY = 0;
            double ratio = 0;
            double angle = 0;
            bool oK = true;

            // tjekker vinklen TIL alle punkter, dvs. fra det andet punkt (nr. 0) og ud
            for (int i = 1; i <= highestIndex; i++)
            {
                deltaX = coords[i].X - coords[i - 1].X;
                deltaY = coords[i].Y - coords[i - 1].Y;

                if (deltaX != 0)
                {
                    if (deltaX > 0)
                    {
                        // deltaX > 0: hvis deltaY er 0 er alt godt, ellers tjekker vi vinklen - NEJ IKKE: og LÆNGDEN - for hvis det er en lille pjosker, gør det ikke noget
                        if (deltaY != 0)
                        {
                            ratio = (double)deltaY / (double)deltaX;
                            angle = System.Math.Atan(ratio) * (180 / Math.PI);

                            // Length = System.Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

                            // if (Length > LengthLimit)
                            oK = oK && (Math.Abs(angle) <= maxAllowedAngle);

                            // Debug.WriteLine($"DeltaX: {DeltaX}, DeltaY: {DeltaY}, Ratio: {Ratio}, Angle: {Angle}, OK: {OK}");
                        }
                    }
                    else
                    {
                        // Debug.WriteLine($"DeltaX < 0");
                        // deltaX < 0; den er helt gal!
                        oK = false;
                    }
                }
                else
                {
                    // Debug.WriteLine($"DeltaX = 0");
                    // deltaX = 0: så skal deltaY OGSÅ være nul - eller meget tæt på... dvs. < +/-5
                    if (deltaY < -5 || deltaY > 5)
                    {
                        oK = false;
                    }
                }
            }

            return oK;
        }

        public static bool CheckBaseLineDirection(string coordsString)
        {
            string temp = coordsString.Replace(" ", ";");
            var pointsArray = temp.Split(';').ToArray();
            int pointsCount = pointsArray.Length;
            bool oK = true;

            if (pointsCount >= 2)
            {
                int previousX = -1;
                int currentX = 0;
                int commaPos = 0;
                for (int i = 0; i < pointsCount; i++)
                {
                    commaPos = pointsArray[i].IndexOf(",");
                    currentX = (int)Convert.ToInt32(pointsArray[i].Substring(0, commaPos));
                    oK = currentX >= previousX;
                    if (oK)
                    {
                        previousX = currentX;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                oK = false;
            }

            return oK;
        }

        public static bool CheckCoordinates(string coordsString, int pageWidth, int pageHeigth)
        {
            TrCoords coords = new TrCoords(coordsString);
            bool oK = true;
            bool xOK;
            bool yOK;

            // tjekker, om et punkt har negative koordinater eller om det er større end siden
            foreach (TrCoord c in coords)
            {
                xOK = c.X >= 0 && c.X <= pageWidth;
                yOK = c.Y >= 0 && c.Y <= pageHeigth;
                oK = oK && xOK && yOK;
            }

            return oK;
        }

        public static bool CheckBaseLineCoordinates(string coordsString)
        {
            TrCoords coords = new TrCoords(coordsString);
            bool oK = true;
            bool xOK;
            bool yOK;

            // tjekker, om et punkt har negative koordinater
            foreach (TrCoord c in coords)
            {
                xOK = c.X >= 0;
                yOK = c.Y >= 0;
                oK = oK && xOK && yOK;
            }

            return oK;
        }

        public static int GetTopYcoord(string coords)
        {
            string temp = coords.Replace(" ", ";");
            var pointsArray = temp.Split(';').ToArray();
            int pointsCount = pointsArray.Length;

            int topY = 20000;

            for (int i = 0; i < pointsCount; i++)
            {
                int commaPos = pointsArray[i].IndexOf(",");
                int y = (int)Convert.ToInt32(pointsArray[i].Substring(commaPos + 1));
                if (y < topY)
                {
                    topY = y;
                }
            }

            return topY;
        }

        public static int GetBottomYcoord(string coords)
        {
            // Debug.WriteLine($"Bottom");
            string temp = coords.Replace(" ", ";");
            var pointsArray = temp.Split(';').ToArray();
            int pointsCount = pointsArray.Length;

            int bottomY = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                int commaPos = pointsArray[i].IndexOf(",");
                int y = (int)Convert.ToInt32(pointsArray[i].Substring(commaPos + 1));
                if (y > bottomY)
                {
                    bottomY = y;
                }

                // Debug.WriteLine($"y = {y}, BottomY= {BottomY}");
            }

            return bottomY;
        }

        public static string RefineText(string rawText, bool convertOtrema)
        {
            string temp = rawText.Trim();
            while (temp.IndexOf("  ") != -1)
            {
                temp = temp.Replace("  ", " ");
            }

            if (convertOtrema)
            {
                temp = temp.Replace('ö', 'ø');
            }

            temp = temp.Replace('ó', 'ø');
            temp = temp.Replace('Ó', 'Ø');
            temp = temp.Replace('ÿ', 'y');
            temp = temp.Replace('ū', 'u');
            temp = temp.Replace('Ū', 'U');
            temp = temp.Replace('ú', 'u');
            temp = temp.Replace('Ú', 'U');

            // flg. fjernet, da de giver problemer med auto-tagging
            //Temp = Temp.Replace('„', '"');
            //Temp = Temp.Replace("\"", "\u201D"); // right double quotation mark
            //Temp = Temp.Replace("\'", "\u2019"); // right single quotation mark

            //Temp = Temp.Replace(" - ", " \u2013 ").Trim();  // en dash
            temp = temp.Replace("...", "\u2026"); // ellipsis

            temp = temp.Replace(" .", ".").Trim();
            temp = temp.Replace(" ,", ",").Trim();

            temp = temp.Replace(" :", ":").Trim();
            temp = temp.Replace(" ;", ";").Trim();

            temp = temp.Replace("( ", "(").Trim();
            temp = temp.Replace(" )", ")").Trim();

            temp = temp.Replace("[ ", "[").Trim();
            temp = temp.Replace(" ]", "]").Trim();

            return temp;
        }

        public static int UniqueNumbersCount(string source)
        {
            List<int> numbers = GetNumbers(source);
            int count = numbers.Count;
            return count;
        }

        public static List<int> GetNumbers(string source)
        {
            string[] numbers = Regex.Split(source, @"\D+");
            List<int> outputList = new List<int>();

            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int i = int.Parse(value);
                    outputList.Add(i);
                }
            }

            return outputList;
        }

        private static int ExpandNumber(int firstNumber, int lastNumber)
        {
            // returnerer 0, hvis der er noget helt galt
            string firstString = firstNumber.ToString();
            string lastString = lastNumber.ToString();
            int firstLength = firstString.Length;
            int lastLength = lastString.Length;

            if (firstNumber < lastNumber && firstLength <= lastLength)
            {
                // alt er godt, numrene er rigtige, fx 1-10, 20-22
                return lastNumber;
            }
            else if (firstNumber > lastNumber && firstLength > lastLength)
            {
                // intervalt fundet af typen 327-29, 1001-8
                int diffLength = firstLength - lastLength;
                string missingNumbers = firstString.Substring(0, diffLength);
                string combinedNumbers = missingNumbers + lastString;
                int newNumber = int.Parse(combinedNumbers);
                return newNumber;
            }
            else if (firstNumber > lastNumber && firstLength == lastLength)
            {
                // her er der noget helt galt, fx 327-315
                return 0;
            }
            else
            {
                return 0;
            }
        }

        public static string GetDate(string source, string year)
        {
            string firstChar = "[";
            string lastChar = "]";
            string dMdivider = "/";
            string mYdivider = "-";
            string result;

            List<int> numbers = GetNumbers(source);
            int count = numbers.Count;

            if (count == 2 || count == 3)   // 3, for det hænder, at nogle også skriver året, fx 2/10-42
            {
                int dayNr = numbers[0];
                int monthNr = numbers[1];
                if (dayNr >= 1 && dayNr <= 31 && monthNr >= 1 && monthNr <= 12)
                {
                    string day = dayNr.ToString();
                    string month = monthNr.ToString();
                    result = firstChar + day + dMdivider + month + mYdivider + year + lastChar;
                }
                else
                {
                    result = "n/a";
                }
            }
            else
            {
                result = "n/a";
            }

            return result;
        }

        public static string ExpandStringWithNumericInterval(string source)
        {
            string result = string.Empty;

            List<int> numbers = GetNumbers(source);
            int count = numbers.Count;

            if (count == 1)
            {
                // ikke noget interval fundet! returnerer input uændret
                result = source;
            }
            else
            {
                int firstNumber = numbers[0];
                int lastNumber = 0;

                if (count == 2)
                {
                    // klassisk interval af formen 317-19 fundet; splitter det op
                    lastNumber = numbers[1];
                }
                else
                {
                    // mere sært interval; sandsynligvis blot oplistning af numre
                    lastNumber = numbers[count - 1];
                }

                lastNumber = ExpandNumber(firstNumber, lastNumber);

                if (firstNumber < lastNumber)
                {
                    // interval fundet, alt er godt (bl.a. har ExpandNumber ikke returneret 0)
                    StringBuilder multipleNumbers = new StringBuilder();
                    for (int i = firstNumber; i <= lastNumber; i++)
                    {
                        string currentNumber = i.ToString();
                        multipleNumbers.Append(currentNumber);
                        if (i < lastNumber)
                        {
                            multipleNumbers.Append(" ");
                        }
                    }

                    result = multipleNumbers.ToString();
                }
                else
                {
                    // enten har ExpandNumber returneret 0, eller også er der noget logisk galt i inputtet
                    result = "n/a";
                }
            }

            return result;
        }

        public static string ExtractRecordName(string source)
        {
            string prefix = "Elfelt-Visit_";
            string suffix = ".tif";

            char lastChar = source[source.Length - 1];
            string temp = string.Empty;

            if (!char.IsLetter(lastChar))
            {
                // der er IKKE et bogstav i enden
                temp = Convert.ToInt32(source).ToString("00000");
            }
            else
            {
                // der ER et bogstav i enden
                //string Letter = NumberMatches[0].Value.Last().ToString();
                int digitCount = source.Length - 1;
                int digitValue = Convert.ToInt32(source.Substring(0, digitCount));
                temp = digitValue.ToString("00000") + lastChar.ToString();
            }

            string outputName = prefix + temp + suffix;
            return outputName;
        }

        public static string GetAccNo(string source, string year)
        {
            // NB: Kan IKKE håndtere efterstillede bogstaver, fx "123a" eller "123a/b"
            string firstChar = "[";
            string lastChar = "]";
            string prefix = "Acc. ";
            string yNdivider = "/";
            string multipleDivider = " - ";
            string result;

            List<int> numbers = GetNumbers(source);
            int count = numbers.Count;

            if (count == 1)
            {
                // simpelt tilfælde: kun eet nummer
                string accNo = numbers[0].ToString();
                result = firstChar + prefix + year + yNdivider + accNo + lastChar;
            }
            else if (count == 2)
            {
                // muligvis et interval af acc-nr.
                int firstNumber = numbers[0];
                int lastNumber = numbers[1];
                lastNumber = ExpandNumber(firstNumber, lastNumber);

                if (firstNumber < lastNumber)
                {
                    // interval fundet
                    // alt er godt (bl.a. har ExpandNumber ikke returneret 0
                    StringBuilder multipleAccNos = new StringBuilder();
                    for (int i = firstNumber; i <= lastNumber; i++)
                    {
                        string currentAccNo = i.ToString();
                        string listValue = prefix + year + yNdivider + currentAccNo;
                        multipleAccNos.Append(listValue);
                        if (i < lastNumber)
                        {
                            multipleAccNos.Append(multipleDivider);
                        }
                    }

                    result = firstChar + multipleAccNos.ToString() + lastChar;
                }
                else
                {
                    // enten har ExpandNumber returneret 0, eller også er der noget logisk galt i inputtet
                    result = "n/a";
                }
            }
            else
            {
                result = "n/a";
            }

            return result;
        }

        public static string StripSharpParanthesis(string source)
        {
            string temp = source;
            temp = temp.Replace('[', ' ');
            temp = temp.Replace(']', ' ');
            temp = temp.Trim();
            return temp;
        }

        public static char GetSuperscriptChar(char source)
        {
            char newChar = NullChar;

            switch (source)
            {
                case '0':
                    newChar = '⁰';
                    break;
                case '1':
                    newChar = '¹';
                    break;
                case '2':
                    newChar = '²';
                    break;
                case '3':
                    newChar = '³';
                    break;
                case '4':
                    newChar = '⁴';
                    break;
                case '5':
                    newChar = '⁵';
                    break;
                case '6':
                    newChar = '⁶';
                    break;
                case '7':
                    newChar = '⁷';
                    break;
                case '8':
                    newChar = '⁸';
                    break;
                case '9':
                    newChar = '⁹';
                    break;
            }

            return newChar;
        }

        public static bool VerifyRegex(string testPattern)
        {
            bool isValid = true;

            if ((testPattern != null) && (testPattern.Trim().Length > 0))
            {
                try
                {
                    Regex.Match(string.Empty, testPattern);
                }
                catch (ArgumentException)
                {
                    // BAD PATTERN: Syntax error
                    isValid = false;
                }
            }
            else
            {
                //BAD PATTERN: Pattern is null or blank
                isValid = false;
            }

            return isValid;
        }
    }
}