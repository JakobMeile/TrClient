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
    public class clsTrTags : IEnumerable
    {
        private List<clsTrTag> Tags;
        public int Count { get => Tags.Count; }

        public clsTrRegion ParentRegion;
        public clsTrTextLine ParentLine;


        public clsTrTags()
        {
            Tags = new List<clsTrTag>();
        }

        public void Add(clsTrTag Tag)
        {
            int TagCountBefore = Tags.Count;

            Tags.Add(Tag);
            Tag.ParentContainer = this;
            Tag.ParentRegion = this.ParentRegion;
            Tag.ParentLine = this.ParentLine;

            int TagCountAfter = Tags.Count;

            //if (Tag.GetType() == typeof(clsTrAbbrevTag))
            //{
            //    Debug.Print($"Added Linetag (abbrev): Expansion = {(Tag as clsTrAbbrevTag).Expansion}, TagCount before = {TagCountBefore}, TagCount after = {TagCountAfter}");
            //}

            //if (Tag.GetType() == typeof(clsTrRomanNumeralTag))
            //{
            //    Debug.Print($"Added roman numeraltag: RomanValue = {(Tag as clsTrRomanNumeralTag).RomanValue}, ArabicEquiv = {(Tag as clsTrRomanNumeralTag).ArabicEquivalent} TagCount before = {TagCountBefore}, TagCount after = {TagCountAfter}");
            //}


        }

        public void LoadFromCustomAttribute(string CustomAttribute)
        {
            string[] TagStrings = CustomAttribute.Split('}').ToArray();
            int TagCount = TagStrings.Length - 1;                       // minus en, da det sidste element er tomt

            for (int i = 0; i < TagCount; i++)
            {
                TagStrings[i] = TagStrings[i].Trim();
                int BracePos = TagStrings[i].IndexOf("{");
                string Type = TagStrings[i].Substring(0, BracePos).Trim();
                string Properties = TagStrings[i].Substring(BracePos + 1).Trim();

                switch (Type)
                {
                    case "readingOrder":
                        //clsTrReadingOrderTag orderTag = new clsTrReadingOrderTag(Type, Properties);
                        //Tags.Add(orderTag);
                        // COMMENTERET ud 27.10.20 - fordi der med disse linier bliver dannet et RO-tag -og det gør der alligevel i construct for region og line!
                        // Debug.WriteLine($"Tag added (ro)"); // - parent = {orderTag.ParentLine.Number}");
                        break;
                    case "structure":
                        clsTrStructuralTag structTag = new clsTrStructuralTag(Type, Properties);
                        Add(structTag);
                        // Debug.WriteLine($"Tag added (struct)");
                        break;

                    case "textStyle":
                        clsTrStyleTag styleTag = new clsTrStyleTag(Properties);
                        Add(styleTag);
                        // Debug.WriteLine($"Tag added (style)");
                        break;

                    case "sic":
                        clsTrSicTag sicTag = new clsTrSicTag(Properties);
                        Add(sicTag);
                        break;

                    case "abbrev":
                        clsTrAbbrevTag abbrevTag = new clsTrAbbrevTag(Properties);
                        Add(abbrevTag);
                        break;

                    case "unclear":
                        clsTrUnclearTag unclearTag = new clsTrUnclearTag(Properties);
                        Add(unclearTag);
                        break;

                    case "date":
                        clsTrDateTag dateTag = new clsTrDateTag(Properties);
                        Add(dateTag);
                        break;

                    case "comment":
                        clsTrCommentTag commentTag = new clsTrCommentTag(Properties);
                        Add(commentTag);
                        break;

                    case "romanNumeral":
                        clsTrRomanNumeralTag romanNumeralTag = new clsTrRomanNumeralTag(Properties);
                        Add(romanNumeralTag);
                        break;

                    default: 
                        // = textual
                        clsTrTextualTag textualTag = new clsTrTextualTag(Type, Properties);
                        Add(textualTag);
                        // Debug.WriteLine($"Tag added (textual)");
                        break;
                }
            }
        }


        public void Clear()
        {
            Tags.Clear();
        }

        public void Sort()
        {
            Tags.Sort();
        }


        public void Move(int StartOffset, int Delta, bool Permanent)
        {
            //Debug.Print($"Page# {ParentLine.ParentPageNr}, Line# {ParentLine.Number}, Text: {ParentLine.TextEquiv}, parameters StartOffset = {StartOffset}, Delta = {Delta}");

            int CurrentOffset;
            int CurrentLength;
            int CurrentEndPosition;

            int NewEndPosition;

            int ParentLineMaxPosition = ParentLine.Length - 1;

            foreach (clsTrTag T in Tags)
            {
                if (T.GetType() == typeof(clsTrTextualTag) || T.GetType().IsSubclassOf(typeof(clsTrTextualTag)))
                {
                    //Debug.Print($"{T.GetType()} : parameters StartOffset = {StartOffset}, Delta = {Delta}");

                    CurrentOffset = (T as clsTrTextualTag).Offset;
                    CurrentLength = (T as clsTrTextualTag).Length;
                    CurrentEndPosition = (T as clsTrTextualTag).EndPosition;

                    //Debug.Print($"CURRENT offset: {CurrentOffset}, length: {CurrentLength}, end: {CurrentEndPosition}");

                    NewEndPosition = CurrentEndPosition + Delta;

                    if (NewEndPosition <= ParentLineMaxPosition || !Permanent)
                    {
                        // hvis hele tagget ligger til højre for start, skal kun dets offset ændres
                        if (CurrentOffset > StartOffset && CurrentEndPosition > StartOffset)
                        {
                            if (Permanent)
                                (T as clsTrTextualTag).Offset = CurrentOffset + Delta;
                            else
                                (T as clsTrTextualTag).DeltaOffset = (T as clsTrTextualTag).DeltaOffset + Delta;
                        }
                        // ellers hvis tagget ligger hen over start, skal kun dets længde ændres
                        else
                        if (CurrentOffset <= StartOffset && CurrentEndPosition >= StartOffset)
                        {
                            if (Permanent)
                                (T as clsTrTextualTag).Length = CurrentLength + Delta;
                            else
                                (T as clsTrTextualTag).DeltaLength = (T as clsTrTextualTag).DeltaLength + Delta;
                        }
                        //Debug.Print($"NEW     offset: {(T as clsTrTextualTag).Offset}, length: {(T as clsTrTextualTag).Length}, end: {(T as clsTrTextualTag).EndPosition}");

                    }
                    else
                    {
                        Debug.Print($"Change not allowed!");

                    }


                }
            }
        }

        public void RemoveAt(int i)
        {
            Tags.RemoveAt(i);
            if (ParentRegion != null)
                ParentRegion.HasChanged = true;
            if (ParentLine != null)
                ParentLine.HasChanged = true;
        }


        public clsTrTag this[int index]
        {
            get { return Tags[index]; }     
            set { Tags[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Tags).GetEnumerator();
        }

        public override string ToString()
        {
            // returnerer "CustomAttributes" - dsv. alle attributter til TextLine, readingorder
            StringBuilder sb = new StringBuilder();

            foreach (clsTrTag T in Tags)
            {
                sb.Append(T.ToString());
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }



    }
}
