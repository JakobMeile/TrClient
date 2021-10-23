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
    public class TrMarcRecord
    {
        private TrParagraphs RecordParagraphs;
        public TrMarcFields Fields = new TrMarcFields();

        public TrMarcRecord(TrParagraphs Paragraphs)
        {
            TrMarcField NewField;
            RecordParagraphs = Paragraphs;
            foreach (TrParagraph P in RecordParagraphs)
            {
                switch (P.Name)
                {
                    // Ordinære felter
                    case "Opstilling":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Opstilling);
                        break;
                    case "Forfatter":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Forfatter);
                        break;
                    case "Titel":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Titel);
                        break;
                    case "Datering":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Datering);
                        break;
                    case "Udgivelse":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Udgivelse);
                        break;
                    case "Beskrivelse":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Beskrivelse);
                        break;
                    case "Indhold":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Indhold);
                        break;
                    case "Indbinding":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Indbinding);
                        break;
                    case "TitelSomEmneord":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.TitelSomEmneord);
                        break;
                    case "Ophav":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Ophav);
                        break;

                    // særlige felter
                    case "BrevAfsender":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.BrevAfsender);
                        break;
                    case "BrevModtager":
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.BrevModtager);
                        break;

                    // udefineret
                    default:
                        NewField = new TrMarcField(P.Content, TrLibrary.MarcFelt.Udefineret);
                        break;
                }
                Fields.Add(NewField);
            }
        }
    }
}
