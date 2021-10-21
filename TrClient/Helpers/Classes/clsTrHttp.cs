using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using TranskribusLibrary;
using TrClient;

namespace TrClient
{
    public class clsTrHttp
    {


        // public static string TrpTranscript = "https://transkribus.eu/TrpServer/rest/collections/_ColID_/_DocID_/_PageNr_/text"; // til UPLOAD


        public static clsTrDocuments MyDocuments = new clsTrDocuments();


        public static clsTrTranscripts MyTranscripts = new clsTrTranscripts();

        public static XmlDocument LinesDocument = new XmlDocument();
        public static TrLines MyLines = new TrLines();

        public static Currents Current = new Currents
        {
            CollectionID = "",
            DocumentID = "",
            DocumentName = "",
            PageNr = "",
            TranscriptID = ""
        };


        // CONSTRUCTOR
        public clsTrHttp()
        {
            
        }








    }
}
