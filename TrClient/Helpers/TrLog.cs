﻿using System;
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
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;
using TrClient.Dialog;

namespace TrClient.Helpers
{
    public class TrLog
    {
        public TrLogEvents Events;

        public string LogFileCaption { get; set; }
        public string LogCollection { get; set; }
        public string LogDocument { get; set; }
        public string LogFileName { get; set; }

        public void Add(TrTextLine Line, string Message)
        {
            TrLogEvent E = new TrLogEvent(Line, Message);
            Events.Add(E);
        }

        public void Add(TrPage Page, string Message)
        {
            TrLogEvent E = new TrLogEvent(Page, Message);
            Events.Add(E);
        }

        public void AddHeader()
        {
            TrLogEvent E = new TrLogEvent();
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

                foreach (TrLogEvent E in Events)
                {
                    LogFile.WriteLine(E.ToString());
                }
            }
        }

        public TrLog(string Caption, string ColName, string DocTitle)
        {
            LogFileCaption = Caption;
            LogCollection = ColName;
            LogDocument = DocTitle;
            // TrLibrary.LogFolder + 
            LogFileName = LogFileCaption.Replace(" ", "") + "_" + ColName + "_" + DocTitle + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";

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