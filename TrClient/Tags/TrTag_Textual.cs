using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Tags
{
    public class TrTag_Textual : TrTag
    {
        private int _offset = 0;
        public int Offset
        {
            get { return _offset + DeltaOffset; }
            set
            {
                if (_offset != value)
                {
                    _offset = value;

                    foreach (TrTagProperty TP in Properties)
                    {
                        if (TP.Name == "offset")
                            TP.Value = _offset.ToString();
                    }
                }
            }
        }

        private int _length = 0;
        public int Length
        {
            get { return _length + DeltaLength; }
            set
            {
                if (_length != value)
                {
                    _length = value;

                    foreach (TrTagProperty TP in Properties)
                    {
                        if (TP.Name == "length")
                            TP.Value = _length.ToString();
                    }
                }
            }
        }

        public TrTag_Textual()
        {
        }

        public TrTag_Textual(string sType, int sOffset, int sLength)
        {
            // constructor for adding NEW tag programmatically

            Type = sType;
            Offset = sOffset;
            Length = sLength;
        }


        public TrTag_Textual(string sType, string sProperties) : base(sType, sProperties)
        {
            // constructor for reading XML files

            string PropName;
            string PropValue;

            Type = sType;

            for (int i = 0; i < Properties.Count; i++)
            {
                PropName = Properties[i].Name;
                PropValue = Properties[i].Value;

                switch (PropName)
                {
                    case "offset":
                        Offset = (int)Convert.ToInt32(PropValue);
                        break;
                    case "length":
                        Length = (int)Convert.ToInt32(PropValue);
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
                _isEmpty = Properties.Count <= 2;
                return _isEmpty;
            }
        }

        private bool _isFull;
        public bool IsFull
        {
            get
            {
                _isFull = (this.Length == ParentLine.Length);
                return _isFull;
            }
        }

        private bool _isSic;
        public bool IsSic
        {
            get
            {
                _isSic = (this.GetType() == typeof(TrTag_Textual_Sic));
                return _isSic;
            }
        }

        private bool _isAbbrev;
        public bool IsAbbrev
        {
            get
            {
                _isAbbrev = (this.GetType() == typeof(TrTag_Textual_Abbrev));
                return _isAbbrev;
            }
        }

        private bool _isUnclear;
        public bool IsUnclear
        {
            get
            {
                _isUnclear = (this.GetType() == typeof(TrTag_Textual_Unclear));
                return _isUnclear;
            }
        }

        private bool _isDate;
        public bool IsDate
        {
            get
            {
                _isDate = (this.GetType() == typeof(TrTag_Textual_Date));
                return _isDate;
            }
        }

        private bool _isStrikethrough;
        public bool IsStrikethrough
        {
            get
            {
                _isStrikethrough = false;
                if (this.GetType() == typeof(TrTag_Textual_Style))
                {
                    if ((this as TrTag_Textual_Style).IsStrikethrough)
                        _isStrikethrough = true;
                }
                return _isStrikethrough;
            }
        }

        private int _endPosition;
        public int EndPosition
        {
            get
            {
                _endPosition = this.Offset + this.Length - 1;
                return _endPosition;
            }
        }

        public new int SortKey
        {
            get
            {
                _sortKey = 100 + Offset - Length;
                return _sortKey;
            }
        }

        private bool _hasOverlappingDateTag;
        public bool HasOverlappingDateTag
        {
            get
            {
                _hasOverlappingDateTag = false;
                foreach (TrTag T in ParentLine.Tags)
                {
                    if (T.GetType() == typeof(TrTag_Textual_Date))
                    {
                        _hasOverlappingDateTag = _hasOverlappingDateTag || ((T as TrTag_Textual_Date).Offset <= this.Offset && (T as TrTag_Textual_Date).EndPosition >= this.EndPosition);
                    }
                }
                return _hasOverlappingDateTag;
            }
        }

        public TrTag_Textual_Date GetOverlappingDateTag()
        {
            TrTag_Textual_Date TempTag = new TrTag_Textual_Date();
            foreach (TrTag T in ParentLine.Tags)
            {
                if (T.GetType() == typeof(TrTag_Textual_Date))
                {
                    if ((T as TrTag_Textual_Date).Offset <= this.Offset && (T as TrTag_Textual_Date).EndPosition >= this.EndPosition)
                    {
                        TempTag = (T as TrTag_Textual_Date);
                        break;
                    }
                }
            }
            return TempTag;
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
