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

namespace TrClient
{
    public class clsTrRow : IComparable, INotifyPropertyChanged
    {
        public clsTrRows ParentContainer;
        public clsTrTextRegion ParentRegion;
        
        public int Number { get; set; }

        public clsTrTextLines Cells = new clsTrTextLines();

        public int CellCount
        {
            get { return Cells.Count;  }
        }


        // constructor 
        public clsTrRow(int RowNumber)
        {
            Number = RowNumber;
        }
               
        public void AddCell(clsTrTextLine C)
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
            var Row = obj as clsTrTextLine;
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

            foreach (clsTrTextLine C in Cells)
            {
                sb.Append(" / ");
                sb.Append(C.TextEquiv);
            }
            return sb.ToString().Trim();

        }

    }
}
