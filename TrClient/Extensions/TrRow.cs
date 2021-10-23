using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using System.IO;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Extensions
{
    public class TrRow : IComparable, INotifyPropertyChanged
    {
        public TrRows ParentContainer;
        public TrRegion_Text ParentRegion;
        
        public int Number { get; set; }

        public TrTextLines Cells = new TrTextLines();

        public int CellCount
        {
            get { return Cells.Count;  }
        }


        // constructor 
        public TrRow(int RowNumber)
        {
            Number = RowNumber;
        }
               
        public void AddCell(TrTextLine C)
        {
            Cells.Add(C);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public int CompareTo(object obj)
        {
            var Row = obj as TrTextLine;
            return Number.CompareTo(Row.Number);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Row #");
            sb.Append(Number.ToString().PadLeft(3));
            sb.Append(" - ");
            sb.Append("Cells:");
            sb.Append(CellCount.ToString().PadLeft(3));
            sb.Append(" - ");
            sb.Append("Content: ");

            foreach (TrTextLine C in Cells)
            {
                sb.Append(" / ");
                sb.Append(C.TextEquiv);
            }
            return sb.ToString().Trim();

        }

    }
}
