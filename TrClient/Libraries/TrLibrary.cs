using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using TrClient;
using System.Globalization;
using System.Text.RegularExpressions;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Libraries
{
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
            BrevModtager        // 700a+h+4
        }

        // SETTINGS
        public static bool OfflineMode = false;
        public static bool LoadOnlyNewestTranscript = true;
        public static bool KOB_ACC = true;  // skal sættes

        public static char CSV_Delimiter = '\t';
        public static char NullChar = '\0';

        //public static string OfflineBaseFolder = @"C:\Users\jakob\Dropbox\KB\Transkribus\SYNC\";

        //public static string ExportFolder = @"C:\Users\jakob\Dropbox\KB\Transkribus\TrClientGUI_Export\";
        //public static string LogFolder = @"C:\Users\jakob\Dropbox\KB\Transkribus\TrClientGUI_Log\";

        public static XNamespace xmlns = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15";
        public static XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public static XNamespace schemaLocation = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15 http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15/pagecontent.xsd";

        public static string TrpBaseAdress = "https://transkribus.eu/TrpServer/rest/";
        public static string TrpLogin = "https://transkribus.eu/TrpServer/rest/auth/login";
        public static string TrpCollections = "https://transkribus.eu/TrpServer/rest/collections/list.xml";

        public static string AppName = "Transkribus Client";

        public static int NarrowColumnWidth = 8;
        public static int BroadColumnWidth = 4 * NarrowColumnWidth;


        public static string EscapeString(string Source)
        {
            string Temp = Source;
            
            // kolon
            Temp = Temp.Replace(":", "\\u003A");

            // semikolon
            Temp = Temp.Replace(":", "\\u003B");

            // højre tuborg
            Temp = Temp.Replace(":", "\\u007D");

            return Temp;
        }

        public static long GetNewTimeStamp()
        {
            long Elapsed = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return Elapsed;
        }

        public static DateTime ConvertUnixTimeStamp(string Timestamp)
        {
            DateTimeOffset dtOffset = new DateTimeOffset();
            dtOffset = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(Timestamp));
            DateTime Date = dtOffset.DateTime;
            return Date;
        }

        public static DateTime ConvertFromISO_Time(string DateString)
        {
            DateTime NewDate = DateTime.ParseExact(DateString, "yyyy-MM-ddThh:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            return NewDate;

        }

        public static string ConvertToISO_Time(DateTime DateTimeObject)
        {
            return DateTimeObject.ToString("yyyy-MM-ddThh:mm:ss.fffzzz");

        }


        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
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
                int Position = temp.IndexOf("readingOrder");
                temp = temp.Substring(Position, temp.Length - Position);
                temp = temp.Replace("readingOrder {index:", "");
                temp = temp.Remove(temp.IndexOf(";"));
                int tempInt = (int)Convert.ToInt32(temp);
                return tempInt + 1;
            }
            else
                return 0;
        }


        public static int GetAverageYcoord(string coords)
        {
            int tempResult = -1;
            if (coords != "")
            {
                string temp = coords.Replace(" ", ";");
                var PointsArray = temp.Split(';').ToArray();
                int PointsCount = PointsArray.Length;

                int SumYcoord = 0;

                for (int i = 0; i < PointsCount; i++)
                {
                    int CommaPos = PointsArray[i].IndexOf(",");
                    int y = (int)Convert.ToInt32(PointsArray[i].Substring(CommaPos + 1));

                    SumYcoord += y;
                }
                tempResult = (int)(SumYcoord / PointsCount);
            }
            return tempResult;
        }

        public static int GetLeftMostXcoord(string coords)
        {
            int tempResult = -1;
            if (coords != "")
            {
                string temp = coords.Replace(" ", ";");
                var PointsArray = temp.Split(';').ToArray();
                int PointsCount = PointsArray.Length;

                int MinX = 10000;

                for (int i = 0; i < PointsCount; i++)
                {
                    int CommaPos = PointsArray[i].IndexOf(",");
                    int x = (int)Convert.ToInt32(PointsArray[i].Substring(0, CommaPos));
                    if (x < MinX)
                        MinX = x;
                }
                tempResult = MinX;
            }
            return tempResult;
        }

        public static int GetRightMostXcoord(string coords)
        {
            string temp = coords.Replace(" ", ";");
            var PointsArray = temp.Split(';').ToArray();
            int PointsCount = PointsArray.Length;

            int MaxX = 0;

            for (int i = 0; i < PointsCount; i++)
            {
                int CommaPos = PointsArray[i].IndexOf(",");
                if (CommaPos > -1)
                {
                    int x = (int)Convert.ToInt32(PointsArray[i].Substring(0, CommaPos));
                    if (x > MaxX)
                        MaxX = x;
                }
            }
            return MaxX;
        }

        public static bool CheckBaseLineStraightness(string CoordsString, double MaxAllowedAngle)
        {
            double LengthLimit = 10;

            TrCoords Coords = new TrCoords(CoordsString);
            int HighestIndex = Coords.Count - 1;
            int DeltaX = 0;
            int DeltaY = 0;
            double Ratio = 0;
            double Angle = 0;
            double Length = 0;
            bool OK = true;

            // tjekker vinklen TIL alle punkter, dvs. fra det andet punkt (nr. 0) og ud
            for (int i = 1; i <= HighestIndex; i++)
            {
                DeltaX = Coords[i].X - Coords[i - 1].X;
                DeltaY = Coords[i].Y - Coords[i - 1].Y;

                if (DeltaX != 0)
                {
                    if (DeltaX > 0)
                    {
                        // deltaX > 0: hvis deltaY er 0 er alt godt, ellers tjekker vi vinklen - NEJ IKKE: og LÆNGDEN - for hvis det er en lille pjosker, gør det ikke noget
                        if (DeltaY != 0)
                        {
                            Ratio = (double)DeltaY / (double)DeltaX;
                            Angle = System.Math.Atan(Ratio) * (180 / Math.PI);
                            // Length = System.Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

                            // if (Length > LengthLimit)
                            OK = OK && (Math.Abs(Angle) <= MaxAllowedAngle);
                            // Debug.WriteLine($"DeltaX: {DeltaX}, DeltaY: {DeltaY}, Ratio: {Ratio}, Angle: {Angle}, OK: {OK}");
                        }
                    }
                    else
                    {
                        // Debug.WriteLine($"DeltaX < 0");
                        // deltaX < 0; den er helt gal!
                        OK = false;
                    }
                }
                else
                {
                    // Debug.WriteLine($"DeltaX = 0");
                    // deltaX = 0: så skal deltaY OGSÅ være nul - eller meget tæt på... dvs. < +/-5
                    if (DeltaY < -5 || DeltaY > 5)
                        OK = false;
                }
            }
            return OK;
        }


        public static bool CheckBaseLineDirection(string CoordsString)
        {
            string temp = CoordsString.Replace(" ", ";");
            var PointsArray = temp.Split(';').ToArray();
            int PointsCount = PointsArray.Length;
            bool OK = true;

            if (PointsCount >= 2)
            {
                int PreviousX = -1;
                int CurrentX = 0;
                int CommaPos = 0;
                for (int i = 0; i < PointsCount; i++)
                {
                    CommaPos = PointsArray[i].IndexOf(",");
                    CurrentX = (int)Convert.ToInt32(PointsArray[i].Substring(0, CommaPos));
                    OK = (CurrentX >= PreviousX);
                    if (OK)
                        PreviousX = CurrentX;
                    else
                        break;
                }
            }
            else
            {
                OK = false;                
            }
            
            return OK;
        }

        public static bool CheckCoordinates(string CoordsString, int PageWidth, int PageHeigth)
        {
            TrCoords Coords = new TrCoords(CoordsString);
            bool OK = true;
            bool xOK;
            bool yOK;

            // tjekker, om et punkt har negative koordinater eller om det er større end siden
            foreach (TrCoord C in Coords)
            {
                xOK = (C.X >= 0 && C.X <= PageWidth);
                yOK = (C.Y >= 0 && C.Y <= PageHeigth);
                OK = OK && xOK && yOK;
            }
            return OK;
        }


        public static bool CheckBaseLineCoordinates(string CoordsString)
        {
            TrCoords Coords = new TrCoords(CoordsString);
            bool OK = true;
            bool xOK;
            bool yOK;

            // tjekker, om et punkt har negative koordinater
            foreach (TrCoord C in Coords)
            {
                xOK = (C.X >= 0);
                yOK = (C.Y >= 0);
                OK = OK && xOK && yOK;
            }
            return OK;
        }


        public static int GetTopYcoord(string coords)
        {
            string temp = coords.Replace(" ", ";");
            var PointsArray = temp.Split(';').ToArray();
            int PointsCount = PointsArray.Length;

            int TopY = 20000;

            for (int i = 0; i < PointsCount; i++)
            {
                int CommaPos = PointsArray[i].IndexOf(",");
                int y = (int)Convert.ToInt32(PointsArray[i].Substring(CommaPos + 1));
                if (y < TopY)
                    TopY = y;
            }
            return TopY;
        }

        public static int GetBottomYcoord(string coords)
        {
            // Debug.WriteLine($"Bottom");
            string temp = coords.Replace(" ", ";");
            var PointsArray = temp.Split(';').ToArray();
            int PointsCount = PointsArray.Length;

            int BottomY = 0;

            for (int i = 0; i < PointsCount; i++)
            {
                int CommaPos = PointsArray[i].IndexOf(",");
                int y = (int)Convert.ToInt32(PointsArray[i].Substring(CommaPos + 1));
                if (y > BottomY)
                    BottomY = y;
                // Debug.WriteLine($"y = {y}, BottomY= {BottomY}");
            }
            return BottomY;
        }

        public static string RefineText(string RawText, bool ConvertOtrema)
        {
            string Temp = RawText.Trim();
            while (Temp.IndexOf("  ") != -1)
            {
                Temp = Temp.Replace("  ", " ");
            }

            if (ConvertOtrema)
                Temp = Temp.Replace('ö', 'ø');

            Temp = Temp.Replace('ó', 'ø');
            Temp = Temp.Replace('Ó', 'Ø');
            Temp = Temp.Replace('ÿ', 'y');
            Temp = Temp.Replace('ū', 'u');
            Temp = Temp.Replace('Ū', 'U');
            Temp = Temp.Replace('ú', 'u');
            Temp = Temp.Replace('Ú', 'U');

            // flg. fjernet, da de giver problemer med auto-tagging
            //Temp = Temp.Replace('„', '"');
            //Temp = Temp.Replace("\"", "\u201D"); // right double quotation mark
            //Temp = Temp.Replace("\'", "\u2019"); // right single quotation mark

            //Temp = Temp.Replace(" - ", " \u2013 ").Trim();  // en dash

            Temp = Temp.Replace("...", "\u2026"); // ellipsis

            Temp = Temp.Replace(" .", ".").Trim();
            Temp = Temp.Replace(" ,", ",").Trim();

            Temp = Temp.Replace(" :", ":").Trim();
            Temp = Temp.Replace(" ;", ";").Trim();

            Temp = Temp.Replace("( ", "(").Trim();
            Temp = Temp.Replace(" )", ")").Trim();

            Temp = Temp.Replace("[ ", "[").Trim();
            Temp = Temp.Replace(" ]", "]").Trim();

            return Temp;
        }

        public static int UniqueNumbersCount(string Source)
        {
            List<int> Numbers = GetNumbers(Source);
            int Count = Numbers.Count;
            return Count;
        }


        public static List<int> GetNumbers(string Source)
        {
            string[] Numbers = Regex.Split(Source, @"\D+");
            List<int> OutputList = new List<int>();

            foreach (string Value in Numbers)
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    int i = int.Parse(Value);
                    OutputList.Add(i);
                }
            }
            return OutputList;
        }

        private static int ExpandNumber(int FirstNumber, int LastNumber)
        {
            // returnerer 0, hvis der er noget helt galt

            string FirstString = FirstNumber.ToString();
            string LastString = LastNumber.ToString();
            int FirstLength = FirstString.Length;
            int LastLength = LastString.Length;

            if (FirstNumber < LastNumber && FirstLength <= LastLength)
            {
                // alt er godt, numrene er rigtige, fx 1-10, 20-22
                return LastNumber;
            }
            else if (FirstNumber > LastNumber && FirstLength > LastLength)
            {
                // intervalt fundet af typen 327-29, 1001-8
                int DiffLength = FirstLength - LastLength;
                string MissingNumbers = FirstString.Substring(0, DiffLength);
                string CombinedNumbers = MissingNumbers + LastString;
                int NewNumber = int.Parse(CombinedNumbers);
                return NewNumber;
            }
	        else if (FirstNumber > LastNumber && FirstLength == LastLength)
            {
                // her er der noget helt galt, fx 327-315
                return 0;
            }
            else
            {
                return 0;
            }
        }



        public static string GetDate(string Source, string Year)
        {
            string FirstChar = "[";
            string LastChar = "]";
            string DMdivider = "/";
            string MYdivider = "-";
            string Result;

            List<int> Numbers = GetNumbers(Source);
            int Count = Numbers.Count;

            if (Count == 2 || Count == 3)   // 3, for det hænder, at nogle også skriver året, fx 2/10-42
            {
                int DayNr = Numbers[0];
                int MonthNr = Numbers[1];
                if (DayNr >= 1 && DayNr <= 31 && MonthNr >= 1 && MonthNr <= 12)
                {
                    string Day = DayNr.ToString();
                    string Month = MonthNr.ToString();
                    Result = FirstChar + Day + DMdivider + Month + MYdivider + Year + LastChar;
                }
                else
                    Result = "n/a";
            }
            else
            {
                Result = "n/a";
            }
            return Result;
        }

        public static string ExpandStringWithNumericInterval(string Source)
        {
            string Result = "";

            List<int> Numbers = GetNumbers(Source);
            int Count = Numbers.Count;

            if (Count == 1)
            {
                // ikke noget interval fundet! returnerer input uændret
                Result = Source;
            }
            else 
            {
                int FirstNumber = Numbers[0];
                int LastNumber = 0;

                if (Count == 2)
                {
                    // klassisk interval af formen 317-19 fundet; splitter det op
                    LastNumber = Numbers[1];
                }
                else
                {
                    // mere sært interval; sandsynligvis blot oplistning af numre
                    LastNumber = Numbers[Count - 1];
                }
                LastNumber = ExpandNumber(FirstNumber, LastNumber);

                if (FirstNumber < LastNumber)
                {
                    // interval fundet, alt er godt (bl.a. har ExpandNumber ikke returneret 0)
                    StringBuilder MultipleNumbers = new StringBuilder();
                    for (int i = FirstNumber; i <= LastNumber; i++)
                    {
                        string CurrentNumber = i.ToString();
                        MultipleNumbers.Append(CurrentNumber);
                        if (i < LastNumber)
                            MultipleNumbers.Append(" ");
                    }
                    Result = MultipleNumbers.ToString();
                }
                else
                {
                    // enten har ExpandNumber returneret 0, eller også er der noget logisk galt i inputtet
                    Result = "n/a";
                }

            }

            return Result;
        }


        public static string ExtractRecordName(string Source)
        {
            string Prefix = "Elfelt-Visit_";
            string Suffix = ".tif";

            char LastChar = Source[Source.Length - 1];
            string Temp = "";

            if (!char.IsLetter(LastChar))
            {
                // der er IKKE et bogstav i enden
                Temp = Convert.ToInt32(Source).ToString("00000");
            }
            else
            {
                // der ER et bogstav i enden
                //string Letter = NumberMatches[0].Value.Last().ToString();
                int DigitCount = Source.Length - 1;
                int DigitValue = Convert.ToInt32(Source.Substring(0, DigitCount));
                Temp = DigitValue.ToString("00000") + LastChar.ToString();

            }
            string OutputName = Prefix + Temp + Suffix;
            return OutputName;
        }


        public static string GetAccNo(string Source, string Year)
        {
            // NB: Kan IKKE håndtere efterstillede bogstaver, fx "123a" eller "123a/b"
            string FirstChar = "[";
            string LastChar = "]";
            string Prefix = "Acc. ";
            string YNdivider = "/";
            string MultipleDivider = " - ";
            string Result;

            List<int> Numbers = GetNumbers(Source);
            int Count = Numbers.Count;

            if (Count == 1)
            {
                // simpelt tilfælde: kun eet nummer
                string AccNo = Numbers[0].ToString();
                Result = FirstChar + Prefix + Year + YNdivider + AccNo + LastChar;
            }
            else if (Count == 2)
            {
                // muligvis et interval af acc-nr.
                int FirstNumber = Numbers[0];
                int LastNumber = Numbers[1];
                LastNumber = ExpandNumber(FirstNumber, LastNumber);

                if (FirstNumber < LastNumber)
                {
                    // interval fundet
                    // alt er godt (bl.a. har ExpandNumber ikke returneret 0
                    StringBuilder MultipleAccNos = new StringBuilder();
                    for (int i = FirstNumber; i <= LastNumber; i++)
                    {
                        string CurrentAccNo = i.ToString();
                        string ListValue = Prefix + Year + YNdivider + CurrentAccNo;
                        MultipleAccNos.Append(ListValue);
                        if (i < LastNumber)
                            MultipleAccNos.Append(MultipleDivider);
                    }
                    Result = FirstChar + MultipleAccNos.ToString() + LastChar;
                }
                else
                {
                    // enten har ExpandNumber returneret 0, eller også er der noget logisk galt i inputtet
                    Result = "n/a";
                }
            }
            else
            {
                Result = "n/a";

            }
            return Result;
        }

        public static string StripSharpParanthesis(string Source)
        {
            string Temp = Source;
            Temp = Temp.Replace('[', ' ');
            Temp = Temp.Replace(']', ' ');
            Temp = Temp.Trim();
            return Temp;
        }

        public static char GetSuperscriptChar(char Source)
        {
            char NewChar = NullChar;

            switch (Source)
            {
                case '0':
                    NewChar = '⁰';
                    break;
                case '1':
                    NewChar = '¹';
                    break;
                case '2':
                    NewChar = '²';
                    break;
                case '3':
                    NewChar = '³';
                    break;
                case '4':
                    NewChar = '⁴';
                    break;
                case '5':
                    NewChar = '⁵';
                    break;
                case '6':
                    NewChar = '⁶';
                    break;
                case '7':
                    NewChar = '⁷';
                    break;
                case '8':
                    NewChar = '⁸';
                    break;
                case '9':
                    NewChar = '⁹';
                    break;
            }
            return NewChar;
        }


        public static bool VerifyRegex(string testPattern)
        {
            bool isValid = true;

            if ((testPattern != null) && (testPattern.Trim().Length > 0))
            {
                try
                {
                    Regex.Match("", testPattern);
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

            return (isValid);
        }
    }
}