using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;


namespace TrClient
{
    public class clsTrMarcField : IComparable
    {

        private string RawText;
        public clsTrLibrary.MarcFelt Number;

        public clsTrMarcFields ParentContainer;
        public clsTrMarcRecord ParentRecord;


        public clsTrMarcField(string Content, clsTrLibrary.MarcFelt FieldType)
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
                case clsTrLibrary.MarcFelt.Opstilling:
                    SubFields = "$$a" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.Forfatter:
                    SubFields = "$$a" + ExtractSurname(RawText) + "$$h" + ExtractGivenName(RawText);
                    break;
                case clsTrLibrary.MarcFelt.Titel:
                    SubFields = "$$a" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.Udgivelse:
                    SubFields = "$$a" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.Datering:
                    SubFields = "$$c" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.Beskrivelse:
                    SubFields = "$$a" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.Indhold:
                    SubFields = "$$a" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.Indbinding:
                    SubFields = "$$a" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.TitelSomEmneord:
                    SubFields = "$$a" + RawText;
                    break;
                case clsTrLibrary.MarcFelt.Ophav:
                    SubFields = "$$a" + RawText;
                    break;

                // særlige felter
                case clsTrLibrary.MarcFelt.BrevAfsender:
                    SubFields = "$$a" + ExtractSurname(RawText) + "$$h" + ExtractGivenName(RawText) + "$$4dkbra";
                    break;
                case clsTrLibrary.MarcFelt.BrevModtager:
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
            var field = obj as clsTrMarcField;
            return Number.CompareTo(field.Number);
        }

    }
}
