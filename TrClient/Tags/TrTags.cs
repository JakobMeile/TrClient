using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class TrTags : IEnumerable
    {
        private List<TrTag> Tags;
        public int Count { get => Tags.Count; }

        public TrRegion ParentRegion;
        public TrTextLine ParentLine;


        public TrTags()
        {
            Tags = new List<TrTag>();
        }

        public void Add(TrTag Tag)
        {
            int TagCountBefore = Tags.Count;

            Tags.Add(Tag);
            Tag.ParentContainer = this;
            Tag.ParentRegion = this.ParentRegion;
            Tag.ParentLine = this.ParentLine;

            int TagCountAfter = Tags.Count;

            //if (Tag.GetType() == typeof(TrTag_Textual_Abbrev))
            //{
            //    Debug.Print($"Added Linetag (abbrev): Expansion = {(Tag as TrTag_Textual_Abbrev).Expansion}, TagCount before = {TagCountBefore}, TagCount after = {TagCountAfter}");
            //}

            //if (Tag.GetType() == typeof(TrTag_Textual_RomanNumeral))
            //{
            //    Debug.Print($"Added roman numeraltag: RomanValue = {(Tag as TrTag_Textual_RomanNumeral).RomanValue}, ArabicEquiv = {(Tag as TrTag_Textual_RomanNumeral).ArabicEquivalent} TagCount before = {TagCountBefore}, TagCount after = {TagCountAfter}");
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
                        //TrTag_ReadingOrder orderTag = new TrTag_ReadingOrder(Type, Properties);
                        //Tags.Add(orderTag);
                        // COMMENTERET ud 27.10.20 - fordi der med disse linier bliver dannet et RO-tag -og det gør der alligevel i construct for region og line!
                        // Debug.WriteLine($"Tag added (ro)"); // - parent = {orderTag.ParentLine.Number}");
                        break;
                    case "structure":
                        TrTag_Structural structTag = new TrTag_Structural(Type, Properties);
                        Add(structTag);
                        // Debug.WriteLine($"Tag added (struct)");
                        break;

                    case "textStyle":
                        TrTag_Textual_Style styleTag = new TrTag_Textual_Style(Properties);
                        Add(styleTag);
                        // Debug.WriteLine($"Tag added (style)");
                        break;

                    case "sic":
                        TrTag_Textual_Sic sicTag = new TrTag_Textual_Sic(Properties);
                        Add(sicTag);
                        break;

                    case "abbrev":
                        TrTag_Textual_Abbrev abbrevTag = new TrTag_Textual_Abbrev(Properties);
                        Add(abbrevTag);
                        break;

                    case "unclear":
                        TrTag_Textual_Unclear unclearTag = new TrTag_Textual_Unclear(Properties);
                        Add(unclearTag);
                        break;

                    case "date":
                        TrTag_Textual_Date dateTag = new TrTag_Textual_Date(Properties);
                        Add(dateTag);
                        break;

                    case "comment":
                        TrTag_Textual_Comment commentTag = new TrTag_Textual_Comment(Properties);
                        Add(commentTag);
                        break;

                    case "romanNumeral":
                        TrTag_Textual_RomanNumeral romanNumeralTag = new TrTag_Textual_RomanNumeral(Properties);
                        Add(romanNumeralTag);
                        break;

                    default: 
                        // = textual
                        TrTag_Textual textualTag = new TrTag_Textual(Type, Properties);
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

            foreach (TrTag T in Tags)
            {
                if (T.GetType() == typeof(TrTag_Textual) || T.GetType().IsSubclassOf(typeof(TrTag_Textual)))
                {
                    //Debug.Print($"{T.GetType()} : parameters StartOffset = {StartOffset}, Delta = {Delta}");

                    CurrentOffset = (T as TrTag_Textual).Offset;
                    CurrentLength = (T as TrTag_Textual).Length;
                    CurrentEndPosition = (T as TrTag_Textual).EndPosition;

                    //Debug.Print($"CURRENT offset: {CurrentOffset}, length: {CurrentLength}, end: {CurrentEndPosition}");

                    NewEndPosition = CurrentEndPosition + Delta;

                    if (NewEndPosition <= ParentLineMaxPosition || !Permanent)
                    {
                        // hvis hele tagget ligger til højre for start, skal kun dets offset ændres
                        if (CurrentOffset > StartOffset && CurrentEndPosition > StartOffset)
                        {
                            if (Permanent)
                                (T as TrTag_Textual).Offset = CurrentOffset + Delta;
                            else
                                (T as TrTag_Textual).DeltaOffset = (T as TrTag_Textual).DeltaOffset + Delta;
                        }
                        // ellers hvis tagget ligger hen over start, skal kun dets længde ændres
                        else
                        if (CurrentOffset <= StartOffset && CurrentEndPosition >= StartOffset)
                        {
                            if (Permanent)
                                (T as TrTag_Textual).Length = CurrentLength + Delta;
                            else
                                (T as TrTag_Textual).DeltaLength = (T as TrTag_Textual).DeltaLength + Delta;
                        }
                        //Debug.Print($"NEW     offset: {(T as TrTag_Textual).Offset}, length: {(T as TrTag_Textual).Length}, end: {(T as TrTag_Textual).EndPosition}");

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


        public TrTag this[int index]
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

            foreach (TrTag T in Tags)
            {
                sb.Append(T.ToString());
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }



    }
}
