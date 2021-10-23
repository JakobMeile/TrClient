using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Helpers
{
    public class TrLogEvent
    {
        public string PageNr { get; set; }
        public string RegionNr { get; set; }
        public string LineNr { get; set; }
        public string Content { get; set; }
        public string LogMessage { get; set; }

        public TrLogEvent(TrTextLine Line, string Message)
        {
            PageNr = Line.ParentRegion.ParentTranscript.ParentPage.PageNr.ToString();
            RegionNr = Line.ParentRegion.Number.ToString();
            LineNr = Line.Number.ToString();
            if (Line.TextEquiv.Length >= TrLibrary.BroadColumnWidth - 3)
                Content = Line.TextEquiv.Substring(0, TrLibrary.BroadColumnWidth - 5) + "...";
            else
                Content = Line.TextEquiv.PadRight(TrLibrary.BroadColumnWidth);
            LogMessage = Message;
        }

        public TrLogEvent(TrPage Page, string Message)
        {
            PageNr = Page.PageNr.ToString();
            RegionNr = "-";
            LineNr = "-";
            Content = "";
            LogMessage = Message;
        }

        public TrLogEvent()
        {

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(160);
            sb.Append(PageNr.PadLeft(TrLibrary.NarrowColumnWidth));
            sb.Append("  ");
            sb.Append(RegionNr.PadLeft(TrLibrary.NarrowColumnWidth));
            sb.Append("  ");
            sb.Append(LineNr.PadLeft(TrLibrary.NarrowColumnWidth));
            sb.Append("  ");
            sb.Append(Content.PadRight(TrLibrary.BroadColumnWidth));
            sb.Append("  ");
            sb.Append(LogMessage);
            return sb.ToString();
        }
    }
}
