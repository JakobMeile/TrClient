// <copyright file="TrCoord.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System;
    using System.Diagnostics;

    public class TrCoord : IComparable
    {
        public int X { get; set; }

        public int Y { get; set; }

        public TrCoord(int newX, int newY)
        {
            X = newX;
            Y = newY;
        }

        public TrCoord(string commaSeparatedXYpair)
        {
            int commaPos = commaSeparatedXYpair.IndexOf(",");
            if (commaPos > 0)
            {
                X = (int)Convert.ToInt32(commaSeparatedXYpair.Substring(0, commaPos));
                Y = (int)Convert.ToInt32(commaSeparatedXYpair.Substring(commaPos + 1));
            }
            else
            {
                Debug.WriteLine("ugyldigt argument til trcoord - tom string i commaseparatepair");
            }
        }

        public TrCoord SubtractOffset(int leftBorder, int topBorder, TrCoord offset)
        {
            int newX = X - leftBorder + offset.X;
            int newY = Y - topBorder + offset.Y;

            TrCoord temp = new TrCoord(newX, newY);
            return temp;
        }

        public int CompareTo(object obj)
        {
            var c = obj as TrCoord;
            return X.CompareTo(c.X);
        }

        public override string ToString()
        {
            string temp = X.ToString() + "," + Y.ToString();
            return temp;
        }
    }
}
