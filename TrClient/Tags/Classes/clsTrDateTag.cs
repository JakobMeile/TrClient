using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using TrClient;
using DanishNLP;

namespace TrClient
{
    public class clsTrDateTag : clsTrTextualTag
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        private bool _resolved = false;
        public bool Resolved
        {
            get { return _resolved; }
            set
            {
                if (_resolved != value)
                {
                    _resolved = value;
                }
            }
        }

        private string _expandedDate = "";
        public string ExpandedDate
        {
            get
            {
                bool ISO = true;
                if (ISO)
                {
                    // ISO format
                    _expandedDate = Year.ToString("0000") + "-" + Month.ToString("00");
                    if (Day > 0)
                        _expandedDate = _expandedDate + "-" + Day.ToString("00");
                }
                else
                {
                    // std. format
                    if (Day > 0)
                        _expandedDate = Day.ToString("00") + "-" + Month.ToString("00") + "-" + Year.ToString("0000");
                    else
                        _expandedDate = clsLanguageLibrary.GetMonthName(Month) + " " + Year.ToString("0000");
                }
                return _expandedDate;
            }
        }

        public clsTrDateTag()
        {
        }

        public clsTrDateTag(int sOffset, int sLength, DateTime sDate) : base("date", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically

            Year = sDate.Year;
            Month = sDate.Month;
            Day = sDate.Day;
        }

        public clsTrDateTag(int sOffset, int sLength, int sDay, int sMonth, int sYear) : base("date", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically

            Year = sYear;
            Month = sMonth;
            Day = sDay;
        }


        public clsTrDateTag(string sProperties) : base("date", sProperties)
        {
            // constructor for reading XML files

            string PropName;
            string PropValue;

            Year = 0;
            Month = 0;
            Day = 0;

            for (int i = 0; i < Properties.Count; i++)
            {
                PropName = Properties[i].Name;
                PropValue = Properties[i].Value;

                switch (PropName)
                {
                    case "year":
                        Year = (int)Convert.ToInt32(PropValue);
                        break;
                    case "month":
                        Month = (int)Convert.ToInt32(PropValue);
                        break;
                    case "day":
                        Day = (int)Convert.ToInt32(PropValue);
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
                _isEmpty = Year > 0 && Month > 0 && Day > 0;
                return _isEmpty;
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
