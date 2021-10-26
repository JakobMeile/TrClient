// <copyright file="TrTags.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core.Tags
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using TrClient.Core;

    public class TrTags : IEnumerable
    {
        private List<TrTag> tags;

        public int Count { get => tags.Count; }

        public TrRegion ParentRegion;
        public TrTextLine ParentLine;

        public TrTags()
        {
            tags = new List<TrTag>();
        }

        public void Add(TrTag tag)
        {
            int tagCountBefore = tags.Count;

            tags.Add(tag);
            tag.ParentContainer = this;
            tag.ParentRegion = ParentRegion;
            tag.ParentLine = ParentLine;

            int tagCountAfter = tags.Count;

            //if (Tag.GetType() == typeof(TrTagTextualAbbrev))
            //{
            //    Debug.Print($"Added Linetag (abbrev): Expansion = {(Tag as TrTagTextualAbbrev).Expansion}, TagCount before = {TagCountBefore}, TagCount after = {TagCountAfter}");
            //}

            //if (Tag.GetType() == typeof(TrTagTextualRomanNumeral))
            //{
            //    Debug.Print($"Added roman numeraltag: RomanValue = {(Tag as TrTagTextualRomanNumeral).RomanValue}, ArabicEquiv = {(Tag as TrTagTextualRomanNumeral).ArabicEquivalent} TagCount before = {TagCountBefore}, TagCount after = {TagCountAfter}");
            //}
        }

        public void LoadFromCustomAttribute(string customAttribute)
        {
            string[] tagStrings = customAttribute.Split('}').ToArray();
            int tagCount = tagStrings.Length - 1;                       // minus en, da det sidste element er tomt

            for (int i = 0; i < tagCount; i++)
            {
                tagStrings[i] = tagStrings[i].Trim();
                int bracePos = tagStrings[i].IndexOf("{");
                string Type = tagStrings[i].Substring(0, bracePos).Trim();
                string Properties = tagStrings[i].Substring(bracePos + 1).Trim();

                switch (Type)
                {
                    case "readingOrder":
                        //TrTagReadingOrder orderTag = new TrTagReadingOrder(Type, Properties);
                        //Tags.Add(orderTag);
                        // COMMENTERET ud 27.10.20 - fordi der med disse linier bliver dannet et RO-tag -og det gør der alligevel i construct for region og line!
                        // Debug.WriteLine($"Tag added (ro)"); // - parent = {orderTag.ParentLine.Number}");
                        break;
                    case "structure":
                        TrTagStructural structTag = new TrTagStructural(Type, Properties);
                        Add(structTag);

                        // Debug.WriteLine($"Tag added (struct)");
                        break;

                    case "textStyle":
                        TrTagTextualStyle styleTag = new TrTagTextualStyle(Properties);
                        Add(styleTag);

                        // Debug.WriteLine($"Tag added (style)");
                        break;

                    case "sic":
                        TrTagTextualSic sicTag = new TrTagTextualSic(Properties);
                        Add(sicTag);
                        break;

                    case "abbrev":
                        TrTagTextualAbbrev abbrevTag = new TrTagTextualAbbrev(Properties);
                        Add(abbrevTag);
                        break;

                    case "unclear":
                        TrTagTextualUnclear unclearTag = new TrTagTextualUnclear(Properties);
                        Add(unclearTag);
                        break;

                    case "date":
                        TrTagTextualDate dateTag = new TrTagTextualDate(Properties);
                        Add(dateTag);
                        break;

                    case "comment":
                        TrTagTextualComment commentTag = new TrTagTextualComment(Properties);
                        Add(commentTag);
                        break;

                    case "romanNumeral":
                        TrTagTextualRomanNumeral romanNumeralTag = new TrTagTextualRomanNumeral(Properties);
                        Add(romanNumeralTag);
                        break;

                    default:
                        // = textual
                        TrTagTextual textualTag = new TrTagTextual(Type, Properties);
                        Add(textualTag);

                        // Debug.WriteLine($"Tag added (textual)");
                        break;
                }
            }
        }

        public void Clear()
        {
            tags.Clear();
        }

        public void Sort()
        {
            tags.Sort();
        }

        public void Move(int startOffset, int delta, bool Permanent)
        {
            //Debug.Print($"Page# {ParentLine.ParentPageNr}, Line# {ParentLine.Number}, Text: {ParentLine.TextEquiv}, parameters StartOffset = {StartOffset}, Delta = {Delta}");
            int currentOffset;
            int currentLength;
            int CurrentEndPosition;

            int NewEndPosition;

            int ParentLineMaxPosition = ParentLine.Length - 1;

            foreach (TrTag T in tags)
            {
                if (T.GetType() == typeof(TrTagTextual) || T.GetType().IsSubclassOf(typeof(TrTagTextual)))
                {
                    //Debug.Print($"{T.GetType()} : parameters StartOffset = {StartOffset}, Delta = {Delta}");
                    currentOffset = (T as TrTagTextual).Offset;
                    currentLength = (T as TrTagTextual).Length;
                    CurrentEndPosition = (T as TrTagTextual).EndPosition;

                    //Debug.Print($"CURRENT offset: {CurrentOffset}, length: {CurrentLength}, end: {CurrentEndPosition}");
                    NewEndPosition = CurrentEndPosition + delta;

                    if (NewEndPosition <= ParentLineMaxPosition || !Permanent)
                    {
                        // hvis hele tagget ligger til højre for start, skal kun dets offset ændres
                        if (currentOffset > startOffset && CurrentEndPosition > startOffset)
                        {
                            if (Permanent)
                            {
                                (T as TrTagTextual).Offset = currentOffset + delta;
                            }
                            else
                            {
                                (T as TrTagTextual).DeltaOffset = (T as TrTagTextual).DeltaOffset + delta;
                            }
                        }

                        // ellers hvis tagget ligger hen over start, skal kun dets længde ændres
                        else
                        if (currentOffset <= startOffset && CurrentEndPosition >= startOffset)
                        {
                            if (Permanent)
                            {
                                (T as TrTagTextual).Length = currentLength + delta;
                            }
                            else
                            {
                                (T as TrTagTextual).DeltaLength = (T as TrTagTextual).DeltaLength + delta;
                            }
                        }

                        //Debug.Print($"NEW     offset: {(T as TrTagTextual).Offset}, length: {(T as TrTagTextual).Length}, end: {(T as TrTagTextual).EndPosition}");
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
            tags.RemoveAt(i);
            if (ParentRegion != null)
            {
                ParentRegion.HasChanged = true;
            }

            if (ParentLine != null)
            {
                ParentLine.HasChanged = true;
            }
        }

        public TrTag this[int index]
        {
            get { return tags[index]; }
            set { tags[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)tags).GetEnumerator();
        }

        public override string ToString()
        {
            // returnerer "CustomAttributes" - dsv. alle attributter til TextLine, readingorder
            StringBuilder sb = new StringBuilder();

            foreach (TrTag t in tags)
            {
                sb.Append(t.ToString());
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }
    }
}
