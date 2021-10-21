using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrCoord : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public clsTrCoord(int newX, int newY)
        {
            X = newX;
            Y = newY;
        }

        public clsTrCoord(string CommaSeparatedXYpair)
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

        public clsTrCoord SubtractOffset(int LeftBorder, int TopBorder, clsTrCoord Offset)
        {
            int NewX = X - LeftBorder + Offset.X;
            int NewY = Y - TopBorder + Offset.Y;


            clsTrCoord Temp = new clsTrCoord(NewX, NewY);
            return Temp;
        }

        public int CompareTo(object obj)
        {
            var c = obj as clsTrCoord;
            return X.CompareTo(c.X);
        }

        public override string ToString()
        {
            string temp = X.ToString() + "," + Y.ToString();
            return temp;
        }
    }
}
