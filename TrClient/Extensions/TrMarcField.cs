using System;
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


namespace TrClient.Extensions
{
    public class TrMarcField : IComparable
    {

        private string RawText;
        public TrLibrary.MarcFelt Number;

        public TrMarcFields ParentContainer;
        public TrMarcRecord ParentRecord;


        public TrMarcField(string Content, TrLibrary.MarcFelt FieldType)
        {
            RawText = Content;
            Number = FieldType;
        }

        public string GetAsText()
        {
            int TempCode = Convert.ToInt32(Number) * 100;
            string FieldCode = TempCode.ToString().PadLeft(5, '0') + " L ";
            string SubFields;

            switch(Number)
            {
                // Ordinære felter
                case TrLibrary.MarcFelt.Opstilling:
                    SubFields = "$$a" + RawText;
                    break;
                case TrLibrary.MarcFelt.Forfatter:
                    SubFields = "$$a" + ExtractSurname(RawText) + "$$h" + ExtractGivenName(RawText);
                    break;
                case TrLibrary.MarcFelt.Titel:
                    SubFields = "$$a" + RawText;
                    break;
                case TrLibrary.MarcFelt.Udgivelse:
                    SubFields = "$$a" + RawText;
                    break;
                case TrLibrary.MarcFelt.Datering:
                    SubFields = "$$c" + RawText;
                    break;
                case TrLibrary.MarcFelt.Beskrivelse:
                    SubFields = "$$a" + RawText;
                    break;
                case TrLibrary.MarcFelt.Indhold:
                    SubFields = "$$a" + RawText;
                    break;
                case TrLibrary.MarcFelt.Indbinding:
                    SubFields = "$$a" + RawText;
                    break;
                case TrLibrary.MarcFelt.TitelSomEmneord:
                    SubFields = "$$a" + RawText;
                    break;
                case TrLibrary.MarcFelt.Ophav:
                    SubFields = "$$a" + RawText;
                    break;

                // særlige felter
                case TrLibrary.MarcFelt.BrevAfsender:
                    SubFields = "$$a" + ExtractSurname(RawText) + "$$h" + ExtractGivenName(RawText) + "$$4dkbra";
                    break;
                case TrLibrary.MarcFelt.BrevModtager:
                    SubFields = "$$a" + ExtractSurname(RawText) + "$$h" + ExtractGivenName(RawText) + "$$4dkbrm";
                    break;

                // udefineret
                default:
                    SubFields = "$$a" + RawText;
                    break;
            }
            return FieldCode + " " + SubFields;
        }

        private string ExtractGivenName(string Content)
        {
            string temp = Content;
            temp = temp.Substring(temp.IndexOf(' '));
            return temp;
        }

        private string ExtractSurname(string Content)
        {
            string temp = DeleteTrailingPunctuation(Content);
            temp = temp.Substring(temp.LastIndexOf(' ') + 1);
            return temp;
        }

        private string DeleteTrailingPunctuation(string Content)
        {
            string temp = Content.Trim();

            string PunctuationMarks = ",.;:?!";
            char Last = temp[temp.Length];
            if (PunctuationMarks.IndexOf(Last) != -1)
                temp = temp.Substring(temp.Length - 1).Trim();
            return temp;
        }


        public int CompareTo(object obj)
        {
            var field = obj as TrMarcField;
            return Number.CompareTo(field.Number);
        }

    }
}
