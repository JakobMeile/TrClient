// <copyright file="TrLog.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using TrClient.Core;
    using TrClient.Views;

    public class TrLog
    {
        public TrLogEvents Events;

        public string LogFileCaption { get; set; }

        public string LogCollection { get; set; }

        public string LogDocument { get; set; }

        public string LogFileName { get; set; }

        public void Add(TrTextLine line, string message)
        {
            TrLogEvent e = new TrLogEvent(line, message);
            Events.Add(e);
        }

        public void Add(TrPage page, string message)
        {
            TrLogEvent e = new TrLogEvent(page, message);
            Events.Add(e);
        }

        public void AddHeader()
        {
            TrLogEvent e = new TrLogEvent();
            e.PageNr = "Page#";
            e.RegionNr = "Region#";
            e.LineNr = "Line#";
            e.Content = "Content:";
            e.LogMessage = "Message:";
            Events.Add(e);
        }

        //-------------------
        public void Add(string eventString)
        {
            Write(eventString);
        }

        public void Add(string eventCaption, int width, string eventMessage)
        {
            string eventString = eventCaption.PadRight(width) + eventMessage;
            Write(eventString);
        }

        public void AddCRLF()
        {
            string eventString = string.Empty;
            Write(eventString);
        }

        public void AddLine()
        {
            StringBuilder sb = new StringBuilder(160);
            for (int i = 1; i <= 120; i++)
            {
                sb.Append("-");
            }

            Write(sb.ToString());
        }

        private void Write(string eventString)
        {
            // LogEvents.Add(EventString);
            // Debug.WriteLine(EventString);
        }

        public void Show()
        {
            ShowLog logWindow = new ShowLog();
            logWindow.lstLogEvents.ItemsSource = Events;

            // LogWindow.Owner = MainWindow.GetWindow(this);
            logWindow.Show();
        }

        public void Save()
        {
            using (StreamWriter logFile = new StreamWriter(LogFileName, true))
            {
                logFile.WriteLine("*** " + LogFileCaption + " ***");
                logFile.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                logFile.WriteLine($"Collection: {LogCollection}");
                logFile.WriteLine($"Document:   {LogDocument}");
                logFile.WriteLine("------------------------------------------------------------------------------------------------------------------------");

                foreach (TrLogEvent e in Events)
                {
                    logFile.WriteLine(e.ToString());
                }
            }
        }

        public TrLog(string caption, string colName, string docTitle)
        {
            LogFileCaption = caption;
            LogCollection = colName;
            LogDocument = docTitle;

            // TrLibrary.LogFolder +
            LogFileName = LogFileCaption.Replace(" ", string.Empty) + "_" + colName + "_" + docTitle + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";

            Events = new TrLogEvents();
            AddHeader();
        }

        //public TrLog(string Caption, string FileName)
        //{
        //    Events = new TrLogEvents();
        //    LogFileCaption = Caption;
        //    LogFileName =  TrLibrary.LogFolder + FileName;
        //    AddLine();
        //    Write("*** " + LogFileCaption + " ***");
        //    AddCRLF();
        //    AddLine();
        //    AddCRLF();

        //}
    }
}
