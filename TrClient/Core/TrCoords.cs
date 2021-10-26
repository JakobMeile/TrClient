// <copyright file="TrCoords.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TrCoords : IEnumerable
    {
        private List<TrCoord> coords;

        public int Count { get => coords.Count; }

        public void Add(TrCoord coord)
        {
            coords.Add(coord);
        }

        public void Sort()
        {
            coords.Sort();
        }

        public void Clear()
        {
            coords.Clear();
        }

        public TrCoord this[int index]
        {
            get { return coords[index]; }
            set { coords[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)coords).GetEnumerator();
        }

        public int GetLeftMostXcoord()
        {
            coords.Sort();
            return coords[0].X;
        }

        public int GetLeftMostYcoord()
        {
            coords.Sort();
            return coords[0].Y;
        }

        public int GetRightMostXcoord()
        {
            coords.Sort();
            return coords[coords.Count - 1].X;
        }

        public int GetRightMostYcoord()
        {
            coords.Sort();
            return coords[coords.Count - 1].Y;
        }

        public TrCoords()
        {
            coords = new List<TrCoord>();
        }

        public TrCoords(string spaceSeparatedListOfCoords)
        {
            string temp = spaceSeparatedListOfCoords.Replace(" ", ";");
            var pointsArray = temp.Split(';').ToArray();
            int pointsCount = pointsArray.Length;

            coords = new List<TrCoord>();

            for (int i = 0; i < pointsCount; i++)
            {
                // Debug.Print($"{i.ToString()} - {PointsArray[i]} - {PointsArray[i].ToString()}");
                TrCoord c = new TrCoord(pointsArray[i].ToString());
                coords.Add(c);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TrCoord c in coords)
            {
                sb.Append(c.ToString());
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }
    }
}
