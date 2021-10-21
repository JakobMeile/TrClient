using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrCoords : IEnumerable
    {
        private List<clsTrCoord> Coords;
        public int Count { get => Coords.Count; }

        public void Add(clsTrCoord Coord)
        {
            Coords.Add(Coord);
        }

        public void Sort()
        {
            Coords.Sort();
        }

        public void Clear()
        {
            Coords.Clear();
        }

        public clsTrCoord this[int index]
        {
            get { return Coords[index]; }
            set { Coords[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Coords).GetEnumerator();
        }

        public int GetLeftMostXcoord()
        {
            Coords.Sort();
            return Coords[0].X;
        }

        public int GetLeftMostYcoord()
        {
            Coords.Sort();
            return Coords[0].Y;
        }

        public int GetRightMostXcoord()
        {
            Coords.Sort();
            return Coords[Coords.Count - 1].X;
        }

        public int GetRightMostYcoord()
        {
            Coords.Sort();
            return Coords[Coords.Count - 1].Y;
        }



        public clsTrCoords()
        {
            Coords = new List<clsTrCoord>();
        }

        public clsTrCoords(string SpaceSeparatedListOfCoords)
        {
            string temp = SpaceSeparatedListOfCoords.Replace(" ", ";");
            var PointsArray = temp.Split(';').ToArray();
            int PointsCount = PointsArray.Length;

            Coords = new List<clsTrCoord>();

            for (int i = 0; i < PointsCount; i++)
            {
                // Debug.Print($"{i.ToString()} - {PointsArray[i]} - {PointsArray[i].ToString()}");
                clsTrCoord C = new clsTrCoord(PointsArray[i].ToString());
                Coords.Add(C);
            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (clsTrCoord C in Coords)
            {
                sb.Append(C.ToString());
                sb.Append(" ");
            }
            return sb.ToString().Trim();

        }
    }
}
