using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using TrClient;
using System.Windows;

namespace TrClient
{
    public class clsTrLog
    {
        public clsTrLogEvents Events;

        public string LogFileCaption { get; set; }
        public string LogCollection { get; set; }
        public string LogDocument { get; set; }
        public string LogFileName { get; set; }

        public void Add(clsTrTextLine Line, string Message)
        {
            clsTrLogEvent E = new clsTrLogEvent(Line, Message);
            Events.Add(E);
        }

        public void Add(clsTrPage Page, string Message)
        {
            clsTrLogEvent E = new clsTrLogEvent(Page, Message);
            Events.Add(E);
        }

        public void AddHeader()
        {
            clsTrLogEvent E = new clsTrLogEvent();
            E.PageNr = "Page#";
            E.RegionNr = "Region#";
            E.LineNr = "Line#";
            E.Content = "Content:";
            E.LogMessage = "Message:";
            Events.Add(E);
        }



        //-------------------
        public void Add(string EventString)
        {
            Write(EventString);
        }

        public void Add(string EventCaption, int Width, string EventMessage)
        {
            string EventString = EventCaption.PadRight(Width) + EventMessage;
            Write(EventString);
        }

        public void AddCRLF()
        {
            string EventString = "";
            Write(EventString);
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

        private void Write(string EventString)
        {
            // LogEvents.Add(EventString);
            // Debug.WriteLine(EventString);
        }

        public void Show()
        {
            dlgShowLog LogWindow = new dlgShowLog();
            LogWindow.lstLogEvents.ItemsSource = Events;
            // LogWindow.Owner = MainWindow.GetWindow(this);
            LogWindow.Show();
        }

        public void Save()
        {
            using (StreamWriter LogFile = new StreamWriter(LogFileName, true))
            {
                LogFile.WriteLine("*** " + LogFileCaption + " ***");
                LogFile.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                LogFile.WriteLine($"Collection: {LogCollection}");
                LogFile.WriteLine($"Document:   {LogDocument}");
                LogFile.WriteLine("------------------------------------------------------------------------------------------------------------------------");

                foreach (clsTrLogEvent E in Events)
                {
                    LogFile.WriteLine(E.ToString());
                }
            }
        }

        public clsTrLog(string Caption, string ColName, string DocTitle)
        {
            LogFileCaption = Caption;
            LogCollection = ColName;
            LogDocument = DocTitle;
            LogFileName = clsTrLibrary.LogFolder + LogFileCaption.Replace(" ", "") + "_" + ColName + "_" + DocTitle + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";

            Events = new clsTrLogEvents();
            AddHeader();
        }

        //public clsTrLog(string Caption, string FileName)
        //{
        //    Events = new clsTrLogEvents();
        //    LogFileCaption = Caption;
        //    LogFileName =  clsTrLibrary.LogFolder + FileName;
        //    AddLine();
        //    Write("*** " + LogFileCaption + " ***");
        //    AddCRLF();
        //    AddLine();
        //    AddCRLF();

        //}
    }
}
