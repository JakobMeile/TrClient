// <copyright file="TrCommon.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains common static functions for project TrClient.
/// </summary>

namespace TranskribusClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Common functions for project TrClient.
    /// </summary>
    public static class TrCommon
    {
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

        public static DateTime ConvertFromISOTime(string dateString)
        {
            DateTime newDate = DateTime.ParseExact(dateString, "yyyy-MM-ddThh:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            return newDate;
        }

        public static string ConvertToISOTime(DateTime dateTimeObject)
        {
            return dateTimeObject.ToString("yyyy-MM-ddThh:mm:ss.fffzzz");
        }
    }
}
