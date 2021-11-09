// <copyright file="TrTagTextual.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core.Tags
{
    using System;
    using System.Text;

    public class TrTagTextual : TrTag
    {
        private int offset = 0;

        public int Offset
        {
            get
            {
                return offset + DeltaOffset;
            }

            set
            {
                if (offset != value)
                {
                    offset = value;

                    foreach (TrTagProperty tP in Properties)
                    {
                        if (tP.Name == "offset")
                        {
                            tP.Value = offset.ToString();
                        }
                    }
                }
            }
        }

        private int length = 0;

        public int Length
        {
            get
            {
                return length + DeltaLength;
            }

            set
            {
                if (length != value)
                {
                    length = value;

                    foreach (TrTagProperty tP in Properties)
                    {
                        if (tP.Name == "length")
                        {
                            tP.Value = length.ToString();
                        }
                    }
                }
            }
        }

        public TrTagTextual()
        {
        }

        public TrTagTextual(string sType, int sOffset, int sLength)
        {
            // constructor for adding NEW tag programmatically
            Type = sType;
            Offset = sOffset;
            Length = sLength;
        }

        public TrTagTextual(string sType, string sProperties)
            : base(sType, sProperties)
        {
            // constructor for reading XML files
            string propName;
            string propValue;

            Type = sType;

            for (int i = 0; i < Properties.Count; i++)
            {
                propName = Properties[i].Name;
                propValue = Properties[i].Value;

                switch (propName)
                {
                    case "offset":
                        Offset = (int)Convert.ToInt32(propValue);
                        break;
                    case "length":
                        Length = (int)Convert.ToInt32(propValue);
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
                // tester antal properties: 2 er minimum - og dermed tom
                isEmpty = Properties.Count <= 2;
                return isEmpty;
            }
        }

        private bool isFull;

        public bool IsFull
        {
            get
            {
                isFull = Length == ParentLine.Length;
                return isFull;
            }
        }

        private bool isSic;

        public bool IsSic
        {
            get
            {
                isSic = GetType() == typeof(TrTagTextualSic);
                return isSic;
            }
        }

        private bool isAbbrev;

        public bool IsAbbrev
        {
            get
            {
                isAbbrev = GetType() == typeof(TrTagTextualAbbrev);
                return isAbbrev;
            }
        }

        private bool isUnclear;

        public bool IsUnclear
        {
            get
            {
                isUnclear = GetType() == typeof(TrTagTextualUnclear);
                return isUnclear;
            }
        }

        private bool isDate;

        public bool IsDate
        {
            get
            {
                isDate = GetType() == typeof(TrTagTextualDate);
                return isDate;
            }
        }

        private bool isStrikethrough;

        public bool IsStrikethrough
        {
            get
            {
                isStrikethrough = false;
                if (GetType() == typeof(TrTagTextualStyle))
                {
                    if ((this as TrTagTextualStyle).IsStrikethrough)
                    {
                        isStrikethrough = true;
                    }
                }

                return isStrikethrough;
            }
        }

        private int endPosition;

        public int EndPosition
        {
            get
            {
                endPosition = Offset + Length - 1;
                return endPosition;
            }
        }

        public bool IsAtIndex(int index)
        {
            return Offset <= index && EndPosition >= index;
        }

        public new int SortKey
        {
            get
            {
                sortKey = 100 + Offset - Length;
                return sortKey;
            }
        }

        private bool hasOverlappingDateTag;

        public bool HasOverlappingDateTag
        {
            get
            {
                hasOverlappingDateTag = false;
                foreach (TrTag t in ParentLine.Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualDate))
                    {
                        hasOverlappingDateTag = hasOverlappingDateTag || ((t as TrTagTextualDate).Offset <= Offset && (t as TrTagTextualDate).EndPosition >= EndPosition);
                    }
                }

                return hasOverlappingDateTag;
            }
        }

        public TrTagTextualDate GetOverlappingDateTag()
        {
            TrTagTextualDate tempTag = new TrTagTextualDate();
            foreach (TrTag t in ParentLine.Tags)
            {
                if (t.GetType() == typeof(TrTagTextualDate))
                {
                    if ((t as TrTagTextualDate).Offset <= Offset && (t as TrTagTextualDate).EndPosition >= EndPosition)
                    {
                        tempTag = t as TrTagTextualDate;
                        break;
                    }
                }
            }

            return tempTag;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Properties.ToString());
            return sb.ToString();
        }
    }
}
