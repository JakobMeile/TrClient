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


namespace TrClient.Core
{
    public class TrCell : IComparable, INotifyPropertyChanged
    {
        public TrCells ParentContainer;
        public TrRegion_Table ParentRegion;
        
        public string ID { get; set; }
        public string CoordsString { get; set; }

        public string CornerPoints { get; set; }

        public string TagString { get; set; }
        public TrTags Tags = new TrTags();
        public TrTag_Structural StructuralTag;

        public TrTextLines TextLines = new TrTextLines();

        public int Row { get; set; }
        public int Col { get; set; }

        private int _sortOrder;
        public int SortOrder
        {
            get
            {
                _sortOrder = Row * 100 + Col;
                return _sortOrder;
            }
        }

        private bool _hasLines;
        public bool HasLines
        {
            get
            {
                _hasLines = (TextLines.Count > 0);
                return _hasLines;
            }
        }


        public void Move(int Horizontally, int Vertically)
        {
            TrCoords C = new TrCoords(CoordsString);
            foreach (TrCoord CurrentCoord in C)
            {
                CurrentCoord.X = CurrentCoord.X + Horizontally;
                CurrentCoord.Y = CurrentCoord.Y + Vertically;
            }
            CoordsString = C.ToString();

            if (HasLines)
            {
                foreach (TrTextLine Line in TextLines)
                {
                    Line.Move(Horizontally, Vertically);
                }
            }
        }



        // constructor 
        public TrCell(string cID, string cRow, string cCol, string cCoords, string cCornerPts)
        {
            ID = cID;
            Row = Convert.ToInt32(cRow);
            Col = Convert.ToInt32(cCol);
            CoordsString = cCoords;
            CornerPoints = cCornerPts;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public int CompareTo(object obj)
        {
            var Cell = obj as TrCell;
            return SortOrder.CompareTo(Cell.SortOrder);
        }



    }
}
