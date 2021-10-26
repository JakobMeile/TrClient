// <copyright file="TrMarcRecord.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Extensions
{
    using TrClient.Libraries;

    public class TrMarcRecord
    {
        private TrParagraphs recordParagraphs;
        public TrMarcFields Fields = new TrMarcFields();

        public TrMarcRecord(TrParagraphs paragraphs)
        {
            TrMarcField newField;
            recordParagraphs = paragraphs;
            foreach (TrParagraph p in recordParagraphs)
            {
                switch (p.Name)
                {
                    // Ordinære felter
                    case "Opstilling":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Opstilling);
                        break;
                    case "Forfatter":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Forfatter);
                        break;
                    case "Titel":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Titel);
                        break;
                    case "Datering":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Datering);
                        break;
                    case "Udgivelse":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Udgivelse);
                        break;
                    case "Beskrivelse":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Beskrivelse);
                        break;
                    case "Indhold":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Indhold);
                        break;
                    case "Indbinding":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Indbinding);
                        break;
                    case "TitelSomEmneord":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.TitelSomEmneord);
                        break;
                    case "Ophav":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Ophav);
                        break;

                    // særlige felter
                    case "BrevAfsender":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.BrevAfsender);
                        break;
                    case "BrevModtager":
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.BrevModtager);
                        break;

                    // udefineret
                    default:
                        newField = new TrMarcField(p.Content, TrLibrary.MarcFelt.Udefineret);
                        break;
                }

                Fields.Add(newField);
            }
        }
    }
}
