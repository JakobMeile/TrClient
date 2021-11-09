// <copyright file="TrMarcField.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Extensions
{
    using System;
    using TranskribusClient.Libraries;

    public class TrMarcField : IComparable
    {
        private string rawText;
        public TrLibrary.MarcFelt Number;

        public TrMarcFields ParentContainer;
        public TrMarcRecord ParentRecord;

        public TrMarcField(string content, TrLibrary.MarcFelt fieldType)
        {
            rawText = content;
            Number = fieldType;
        }

        public string GetAsText()
        {
            int tempCode = Convert.ToInt32(Number) * 100;
            string fieldCode = tempCode.ToString().PadLeft(5, '0') + " L ";
            string subFields;

            switch (Number)
            {
                // Ordinære felter
                case TrLibrary.MarcFelt.Opstilling:
                    subFields = "$$a" + rawText;
                    break;
                case TrLibrary.MarcFelt.Forfatter:
                    subFields = "$$a" + ExtractSurname(rawText) + "$$h" + ExtractGivenName(rawText);
                    break;
                case TrLibrary.MarcFelt.Titel:
                    subFields = "$$a" + rawText;
                    break;
                case TrLibrary.MarcFelt.Udgivelse:
                    subFields = "$$a" + rawText;
                    break;
                case TrLibrary.MarcFelt.Datering:
                    subFields = "$$c" + rawText;
                    break;
                case TrLibrary.MarcFelt.Beskrivelse:
                    subFields = "$$a" + rawText;
                    break;
                case TrLibrary.MarcFelt.Indhold:
                    subFields = "$$a" + rawText;
                    break;
                case TrLibrary.MarcFelt.Indbinding:
                    subFields = "$$a" + rawText;
                    break;
                case TrLibrary.MarcFelt.TitelSomEmneord:
                    subFields = "$$a" + rawText;
                    break;
                case TrLibrary.MarcFelt.Ophav:
                    subFields = "$$a" + rawText;
                    break;

                // særlige felter
                case TrLibrary.MarcFelt.BrevAfsender:
                    subFields = "$$a" + ExtractSurname(rawText) + "$$h" + ExtractGivenName(rawText) + "$$4dkbra";
                    break;
                case TrLibrary.MarcFelt.BrevModtager:
                    subFields = "$$a" + ExtractSurname(rawText) + "$$h" + ExtractGivenName(rawText) + "$$4dkbrm";
                    break;

                // udefineret
                default:
                    subFields = "$$a" + rawText;
                    break;
            }

            return fieldCode + " " + subFields;
        }

        private string ExtractGivenName(string content)
        {
            string temp = content;
            temp = temp.Substring(temp.IndexOf(' '));
            return temp;
        }

        private string ExtractSurname(string content)
        {
            string temp = DeleteTrailingPunctuation(content);
            temp = temp.Substring(temp.LastIndexOf(' ') + 1);
            return temp;
        }

        private string DeleteTrailingPunctuation(string content)
        {
            string temp = content.Trim();

            string punctuationMarks = ",.;:?!";
            char last = temp[temp.Length];
            if (punctuationMarks.IndexOf(last) != -1)
            {
                temp = temp.Substring(temp.Length - 1).Trim();
            }

            return temp;
        }

        public int CompareTo(object obj)
        {
            var field = obj as TrMarcField;
            return Number.CompareTo(field.Number);
        }
    }
}
