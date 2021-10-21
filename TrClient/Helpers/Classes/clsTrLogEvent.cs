using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrClient
{
    public class clsTrLogEvent
    {
        public string PageNr { get; set; }
        public string RegionNr { get; set; }
        public string LineNr { get; set; }
        public string Content { get; set; }
        public string LogMessage { get; set; }

        public clsTrLogEvent(clsTrTextLine Line, string Message)
        {
            PageNr = Line.ParentRegion.ParentTranscript.ParentPage.PageNr.ToString();
            RegionNr = Line.ParentRegion.Number.ToString();
            LineNr = Line.Number.ToString();
            if (Line.TextEquiv.Length >= clsTrLibrary.BroadColumnWidth - 3)
                Content = Line.TextEquiv.Substring(0, clsTrLibrary.BroadColumnWidth - 5) + "...";
            else
                Content = Line.TextEquiv.PadRight(clsTrLibrary.BroadColumnWidth);
            LogMessage = Message;
        }

        public clsTrLogEvent(clsTrPage Page, string Message)
        {
            PageNr = Page.PageNr.ToString();
            RegionNr = "-";
            LineNr = "-";
            Content = "";
            LogMessage = Message;
        }

        public clsTrLogEvent()
        {

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(160);
            sb.Append(PageNr.PadLeft(clsTrLibrary.NarrowColumnWidth));
            sb.Append("  ");
            sb.Append(RegionNr.PadLeft(clsTrLibrary.NarrowColumnWidth));
            sb.Append("  ");
            sb.Append(LineNr.PadLeft(clsTrLibrary.NarrowColumnWidth));
            sb.Append("  ");
            sb.Append(Content.PadRight(clsTrLibrary.BroadColumnWidth));
            sb.Append("  ");
            sb.Append(LogMessage);
            return sb.ToString();
        }
    }
}
