using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;


namespace TrClient
{
    public class clsTrMarcRecord
    {
        private clsTrParagraphs RecordParagraphs;
        public clsTrMarcFields Fields = new clsTrMarcFields();

        public clsTrMarcRecord(clsTrParagraphs Paragraphs)
        {
            clsTrMarcField NewField;
            RecordParagraphs = Paragraphs;
            foreach (clsTrParagraph P in RecordParagraphs)
            {
                switch (P.Name)
                {
                    // Ordinære felter
                    case "Opstilling":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Opstilling);
                        break;
                    case "Forfatter":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Forfatter);
                        break;
                    case "Titel":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Titel);
                        break;
                    case "Datering":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Datering);
                        break;
                    case "Udgivelse":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Udgivelse);
                        break;
                    case "Beskrivelse":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Beskrivelse);
                        break;
                    case "Indhold":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Indhold);
                        break;
                    case "Indbinding":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Indbinding);
                        break;
                    case "TitelSomEmneord":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.TitelSomEmneord);
                        break;
                    case "Ophav":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Ophav);
                        break;

                    // særlige felter
                    case "BrevAfsender":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.BrevAfsender);
                        break;
                    case "BrevModtager":
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.BrevModtager);
                        break;

                    // udefineret
                    default:
                        NewField = new clsTrMarcField(P.Content, clsTrLibrary.MarcFelt.Udefineret);
                        break;
                }
                Fields.Add(NewField);
            }
        }
    }
}
