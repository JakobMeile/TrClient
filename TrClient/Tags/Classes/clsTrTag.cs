using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public abstract class clsTrTag : IComparable
    {
        public string Type { get; set; }
        public clsTrTagProperties Properties = new clsTrTagProperties();

        public clsTrTags ParentContainer { get; set; }

        public clsTrRegion ParentRegion { get; set; }
        public clsTrTextLine ParentLine { get; set; }

        public bool MarkToDeletion = false;

        public int DeltaOffset { get; set; }
        public int DeltaLength { get; set; }


        public clsTrTag(string sType, string sProperties) : base()
        {
            // constructor for reading XML files

            Type = sType;
            string[] TempArray = sProperties.Split(';').ToArray();
            int Count = TempArray.Length - 1;                       // minus en, da det sidste element er tomt

            for (int i = 0; i < Count; i++)
            {
                clsTrTagProperty P = new clsTrTagProperty(TempArray[i]);
                Properties.Add(P);
            }
        }

        public clsTrTag()
        {
            DeltaOffset = 0;
            DeltaLength = 0;
            SortKey = 0;
        }


        public override string ToString()
        {
            string Temp = Type + " " + Properties.ToString();
            return Temp;
        }

        protected bool _isEmpty;
        public abstract bool IsEmpty { get; }

        protected int _sortKey;
        public int SortKey { get; set; }

        public int CompareTo(object obj)
        {
            var tag = obj as clsTrTextualTag;
            return SortKey.CompareTo(tag.SortKey);
        }

    }
}
