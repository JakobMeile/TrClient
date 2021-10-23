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


        public TrTag(string sType, string sProperties) : base()
        {
            // constructor for reading XML files

            Type = sType;
            string[] TempArray = sProperties.Split(';').ToArray();
            int Count = TempArray.Length - 1;                       // minus en, da det sidste element er tomt

            for (int i = 0; i < Count; i++)
            {
                TrTagProperty P = new TrTagProperty(TempArray[i]);
                Properties.Add(P);
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
            string Temp = Type + " " + Properties.ToString();
            return Temp;
        }

        protected bool _isEmpty;
        public abstract bool IsEmpty { get; }

        protected int _sortKey;
        public int SortKey { get; set; }

        public int CompareTo(object obj)
        {
            var tag = obj as TrTag_Textual;
            return SortKey.CompareTo(tag.SortKey);
        }

    }
}
