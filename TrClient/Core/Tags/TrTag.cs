// <copyright file="TrTag.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core.Tags
{
    using System;
    using System.Linq;
    using TrClient.Core;

    public abstract class TrTag : IComparable
    {
        public string Type { get; set; }

        public TrTagProperties Properties = new TrTagProperties();

        public TrTags ParentContainer { get; set; }

        public TrRegion ParentRegion { get; set; }

        public TrTextLine ParentLine { get; set; }

        public bool MarkToDeletion = false;

        public int DeltaOffset { get; set; }

        public int DeltaLength { get; set; }

        public TrTag(string sType, string sProperties)
            : base()
        {
            // constructor for reading XML files
            Type = sType;
            string[] tempArray = sProperties.Split(';').ToArray();
            int count = tempArray.Length - 1;                       // minus en, da det sidste element er tomt

            for (int i = 0; i < count; i++)
            {
                TrTagProperty p = new TrTagProperty(tempArray[i]);
                Properties.Add(p);
            }
        }

        public TrTag()
        {
            DeltaOffset = 0;
            DeltaLength = 0;
            SortKey = 0;
        }

        public override string ToString()
        {
            string temp = Type + " " + Properties.ToString();
            return temp;
        }

        protected bool isEmpty;

        public abstract bool IsEmpty { get; }

        protected int sortKey;

        public int SortKey { get; set; }

        public int CompareTo(object obj)
        {
            var tag = obj as TrTagTextual;
            return SortKey.CompareTo(tag.SortKey);
        }
    }
}
