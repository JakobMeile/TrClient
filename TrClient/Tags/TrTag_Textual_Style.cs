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
    public class TrTag_Textual_Style : TrTag_Textual
    {

        public float FontSize { get; set; }
        public float Kerning { get; set; } 

        public bool Superscript { get; set; } 
        public bool Subscript { get; set; } 

        public bool Bold { get; set; } 
        public bool Italic { get; set; } 

        public bool Underlined { get; set; } 
        public bool Strikethrough { get; set; } 

        public bool SmallCaps { get; set; } 
        public bool Serif { get; set; } 
        public bool LetterSpaced { get; set; } 
        public bool Monospace { get; set; } 
        public bool ReverseVideo { get; set; }


        public TrTag_Textual_Style(int sOffset, int sLength, string sType) : base("textStyle", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            // nulstilling af properties:
            FontSize = 0;
            Kerning = 0;
            Superscript = false;
            Subscript = false;
            Bold = false;
            Italic = false;
            Underlined = false;
            Strikethrough = false;
            SmallCaps = false;
            Serif = false;
            LetterSpaced = false;
            Monospace = false;
            ReverseVideo = false;

            switch (sType)
            {
                case "superscript":
                    Superscript = true;
                    break;
                case "subscript":
                    Subscript = true;
                    break;
                case "bold":
                    Bold = true;
                    break;
                case "italic":
                    Italic = true;
                    break;
                case "underlined":
                    Underlined = true;
                    break;
                case "strikethrough":
                    Strikethrough = true;
                    break;
                case "smallCaps":
                    SmallCaps = true;
                    break;
                case "serif":
                    Serif = true;
                    break;
                case "letterSpaced":
                    LetterSpaced = true;
                    break;
                case "monospace":
                    Monospace = true;
                    break;
                case "reverseVideo":
                    ReverseVideo = true;
                    break;
            }
        }

        public TrTag_Textual_Style(string sProperties) : base("textStyle", sProperties)
        {
            // constructor for reading XML files
            // nulstilling af properties:
            FontSize = 0;
            Kerning = 0;
            Superscript = false;
            Subscript = false;
            Bold = false;
            Italic = false;
            Underlined = false;
            Strikethrough = false;
            SmallCaps = false;
            Serif = false;
            LetterSpaced = false;
            Monospace = false;
            ReverseVideo = false;


            for (int i = 0; i < Properties.Count; i++)
            {
                if (Properties[i].Name == "fontSize")
                    FontSize = (float)Convert.ToDouble(Properties[i].Value);
                else if (Properties[i].Name == "kerning")
                    Kerning = (float)Convert.ToDouble(Properties[i].Value);
                else
                {
                    if (Properties[i].Value == "true")
                        switch (Properties[i].Name)
                        {
                            case "superscript":
                                Superscript = true;
                                break;
                            case "subscript":
                                Subscript = true;
                                break;
                            case "bold":
                                Bold = true;
                                break;
                            case "italic":
                                Italic = true;
                                break;
                            case "underlined":
                                Underlined = true;
                                break;
                            case "strikethrough": 
                                Strikethrough = true;
                                break;
                            case "smallCaps":
                                SmallCaps = true;
                                break;
                            case "serif":
                                Serif = true;
                                break;
                            case "letterSpaced":
                                LetterSpaced = true;
                                break;
                            case "monospace":
                                Monospace = true;
                                break;
                            case "reverseVideo":
                                ReverseVideo = true;
                                break;
                        }
                }

            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Type);
            sb.Append(" {");

            sb.Append("offset:");
            sb.Append(Offset.ToString());
            sb.Append("; ");

            sb.Append("length:");
            sb.Append(Length.ToString());
            sb.Append("; ");

            sb.Append("fontSize:");
            sb.Append(FontSize.ToString());
            sb.Append("; ");

            sb.Append("kerning:");
            sb.Append(Kerning.ToString());
            sb.Append("; ");

            if (Superscript)
                sb.Append("superscript:true; ");

            if (Subscript)
                sb.Append("subscript:true; ");

            if (Bold)
                sb.Append("bold:true; ");

            if (Italic)
                sb.Append("italic:true; ");

            if (Underlined)
                sb.Append("underlined:true; ");

            if (Strikethrough)
                sb.Append("strikethrough:true; ");

            if (SmallCaps)
                sb.Append("smallCaps:true; ");

            if (Serif)
                sb.Append("serif:true; ");

            if (LetterSpaced)
                sb.Append("letterSpaced:true; ");

            if (Monospace)
                sb.Append("monospace:true; ");

            if (ReverseVideo)
                sb.Append("reverseVideo:true; ");

            sb.Append("}");

            return sb.ToString();
        }

    }
}
