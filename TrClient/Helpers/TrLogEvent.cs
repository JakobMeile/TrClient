// <copyright file="TrLogEvent.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Helpers
{
    using System.Text;
    using TrClient.Core;
    using TrClient.Libraries;

    public class TrLogEvent
    {
        public string PageNr { get; set; }

        public string RegionNr { get; set; }

        public string LineNr { get; set; }

        public string Content { get; set; }

        public string LogMessage { get; set; }

        public TrLogEvent(TrTextLine line, string message)
        {
            PageNr = line.ParentRegion.ParentTranscript.ParentPage.PageNr.ToString();
            RegionNr = line.ParentRegion.Number.ToString();
            LineNr = line.Number.ToString();
            if (line.TextEquiv.Length >= TrLibrary.BroadColumnWidth - 3)
            {
                Content = line.TextEquiv.Substring(0, TrLibrary.BroadColumnWidth - 5) + "...";
            }
            else
            {
                Content = line.TextEquiv.PadRight(TrLibrary.BroadColumnWidth);
            }

            LogMessage = message;
        }

        public TrLogEvent(TrPage page, string message)
        {
            PageNr = page.PageNr.ToString();
            RegionNr = "-";
            LineNr = "-";
            Content = string.Empty;
            LogMessage = message;
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
