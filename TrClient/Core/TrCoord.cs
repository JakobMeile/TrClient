using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Core
{
    public class TrCoord : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public TrCoord(int newX, int newY)
        {
            X = newX;
            Y = newY;
        }

        public TrCoord(string CommaSeparatedXYpair)
        {
            int CommaPos = CommaSeparatedXYpair.IndexOf(",");
            if (CommaPos > 0)
            {
                X = (int)Convert.ToInt32(CommaSeparatedXYpair.Substring(0, CommaPos));
                Y = (int)Convert.ToInt32(CommaSeparatedXYpair.Substring(CommaPos + 1));
            }
            else
                Debug.WriteLine("ugyldigt argument til trcoord - tom string i commaseparatepair");

        }

        public TrCoord SubtractOffset(int LeftBorder, int TopBorder, TrCoord Offset)
        {
            int NewX = X - LeftBorder + Offset.X;
            int NewY = Y - TopBorder + Offset.Y;


            TrCoord Temp = new TrCoord(NewX, NewY);
            return Temp;
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
