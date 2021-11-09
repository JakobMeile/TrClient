// <copyright file="TrTagTextualDate.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Core.Tags
{
    using System;
    using System.Text;
    using DanishNLP;

    public class TrTagTextualDate : TrTagTextual
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        private bool resolved = false;

        public bool Resolved
        {
            get
            {
                return resolved;
            }

            set
            {
                if (resolved != value)
                {
                    resolved = value;
                }
            }
        }

        private string expandedDate = string.Empty;

        public string ExpandedDate
        {
            get
            {
                bool iSO = true;
                if (iSO)
                {
                    // ISO format
                    expandedDate = Year.ToString("0000") + "-" + Month.ToString("00");
                    if (Day > 0)
                    {
                        expandedDate = expandedDate + "-" + Day.ToString("00");
                    }
                }
                else
                {
                    // std. format
                    if (Day > 0)
                    {
                        expandedDate = Day.ToString("00") + "-" + Month.ToString("00") + "-" + Year.ToString("0000");
                    }
                    else
                    {
                        expandedDate = ClsLanguageLibrary.GetMonthName(Month) + " " + Year.ToString("0000");
                    }
                }

                return expandedDate;
            }
        }

        public TrTagTextualDate()
        {
        }

        public TrTagTextualDate(int sOffset, int sLength, DateTime sDate)
            : base("date", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            Year = sDate.Year;
            Month = sDate.Month;
            Day = sDate.Day;
        }

        public TrTagTextualDate(int sOffset, int sLength, int sDay, int sMonth, int sYear)
            : base("date", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            Year = sYear;
            Month = sMonth;
            Day = sDay;
        }

        public TrTagTextualDate(string sProperties)
            : base("date", sProperties)
        {
            // constructor for reading XML files
            string propName;
            string propValue;

            Year = 0;
            Month = 0;
            Day = 0;

            for (int i = 0; i < Properties.Count; i++)
            {
                propName = Properties[i].Name;
                propValue = Properties[i].Value;

                switch (propName)
                {
                    case "year":
                        Year = (int)Convert.ToInt32(propValue);
                        break;
                    case "month":
                        Month = (int)Convert.ToInt32(propValue);
                        break;
                    case "day":
                        Day = (int)Convert.ToInt32(propValue);
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool IsEmpty
        {
            get
            {
                // omvendt test - nemmest - og returnerer "not test"
                isEmpty = Year > 0 && Month > 0 && Day > 0;
                return isEmpty;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Type);
            sb.Append(" {");

            sb.Append("offset:");
            sb.Append(Offset.ToString());
            sb.Append("; ");

            sb.Append("length:");
            sb.Append(Length.ToString());
            sb.Append("; ");

            // lidt særligt format
            // date {offset:0; length:3;year:1900; day:4; month:2;}
            // date {offset:0; length:1;year:0; day:0; month:0;}
            sb.Append("year:");
            sb.Append(Year.ToString());
            sb.Append("; ");

            sb.Append("day:");
            sb.Append(Day.ToString());
            sb.Append("; ");

            sb.Append("month:");
            sb.Append(Month.ToString());
            sb.Append("; ");

            sb.Append("}");

            return sb.ToString();
        }
    }
}
