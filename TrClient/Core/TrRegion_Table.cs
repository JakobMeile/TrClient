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
    public class TrRegion_Table : TrRegion
    {
        public TrCells Cells = new TrCells();

        // OVERRIDE PROPERTIES ----------------------------------------------------------------------

        public override bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                if (_changesUploaded)
                    StatusColor = Brushes.DarkViolet;
                NotifyPropertyChanged("ChangesUploaded");
                foreach (TrCell Cell in Cells)
                    Cell.TextLines.ChangesUploaded = value;          // OVERRIDE pga denne linie
            }
        }

        public override bool HasLines
        {
            get
            {
                bool temp = false;
                if (Cells.Count > 0)
                {
                    foreach (TrCell Cell in Cells)
                        temp = temp || (Cell.TextLines.Count > 0);
                }
                return temp;
            }
        }

        private int _numberOfLines = 0;
        public override int NumberOfLines
        {
            get
            {
                int temp = 0;
                if (Cells.Count > 0)
                {
                    foreach (TrCell Cell in Cells)
                        temp = temp + Cell.TextLines.Count;
                }
                _numberOfLines = temp;
                return _numberOfLines;
            }
        }



        // OVERRIDE METHODS ------------------------------------------------------------------------------------

        public override List<string> GetStructuralTags()
        {
            List<string> TempList = new List<string>();
            foreach (TrCell Cell in Cells)
            {
                foreach (TrTextLine TL in Cell.TextLines)        // OVERRIDE pga denne line
                    if (TL.HasStructuralTag)
                        TempList.Add(TL.StructuralTagValue);
            }

            List<string> TagList = TempList.Distinct().ToList();
            TagList.Sort();
            return TagList;
        }

        public override void Move(int Horizontally, int Vertically)
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
                foreach (TrCell Cell in Cells)
                {
                    Cell.Move(Horizontally, Vertically);
                }

            }
        }

        public override bool DeleteShortBaselines(int Limit, TrLog Log)
        {
            bool RegionIsOK = true;
            bool CellIsOK = true;
            string ErrorMessage;

            foreach (TrCell Cell in Cells)
            {
                foreach (TrTextLine Line in Cell.TextLines)
                {
                    if (Line.Width < Limit)
                    {
                        CellIsOK = false;
                        Line.MarkToDeletion = true;
                        ErrorMessage = $"Width = {Line.Width}: Line deleted!";
                        Log.Add(Line, ErrorMessage);
                    }
                }
                if (!CellIsOK)
                    for (int i = Cell.TextLines.Count - 1; i >= 0; i--)
                    {
                        if (Cell.TextLines[i].MarkToDeletion)
                            Cell.TextLines.RemoveAt(i);
                    }
                RegionIsOK = RegionIsOK && CellIsOK;

            }
            return RegionIsOK;
        }

        public override void SimplifyBoundingBoxes()
        {
            foreach (TrCell Cell in Cells)
            {
                foreach (TrTextLine Line in Cell.TextLines)
                    Line.SimplifyBoundingBox();
            }
        }

        public override void SimplifyBoundingBoxes(int MinimumHeight, int MaximumHeight)
        {
            foreach (TrCell Cell in Cells)
            {
                foreach (TrTextLine Line in Cell.TextLines)
                    Line.SimplifyBoundingBox(MinimumHeight, MaximumHeight);
            }
        }

        public override List<string> GetExpandedText(bool Refine, bool ConvertOtrema)
        {
            List<string> TempList = new List<string>();

            foreach (TrCell Cell in Cells)
            {
                foreach (TrTextLine TL in Cell.TextLines)
                    TempList.Add(TL.ExpandedText);
            }

            return TempList;
        }

        public override TrWords Words
        {
            get
            {
                foreach (TrCell Cell in Cells)
                {
                    foreach (TrTextLine TL in Cell.TextLines)
                    {
                        foreach (TrWord W in TL.Words)
                            _words.Add(W);
                    }
                }

                return _words;
            }
        }

        public override void ExtendBaseLines(TrDialogTransferSettings Settings, TrLog Log)
        {
            // Debug.WriteLine($"TrRegion_Text : ExtendBaseLines");

            string ErrorMessage;

            if (HasLines)
            {
                foreach (TrCell Cell in Cells)
                {
                    foreach (TrTextLine Line in Cell.TextLines)
                    {
                        if (Settings.ExtendLeft)
                        {
                            Line.ExtendLeft(Settings.LeftAmount);
                            ErrorMessage = $"Line extended left with {Settings.LeftAmount}!";
                            Log.Add(Line, ErrorMessage);
                        }
                        if (Settings.ExtendRight)
                        {
                            Line.ExtendRight(Settings.RightAmount);
                            ErrorMessage = $"Line extended right with {Settings.RightAmount}!";
                            Log.Add(Line, ErrorMessage);
                        }
                    }
                }

            }
        }
        

        public override XElement ToXML()
        {
            string CustomString = Tags.ToString();

            XElement xRegion = new XElement(TrLibrary.xmlns + "TableRegion",
                new XAttribute("type", Type.ToString()),
                new XAttribute("orientation", Orientation.ToString()),
                new XAttribute("id", ID),
                new XAttribute("custom", CustomString),
                new XElement(TrLibrary.xmlns + "Coords",
                new XAttribute("points", CoordsString)));

            StringBuilder sb = new StringBuilder();

            foreach (TrCell Cell in Cells)
            {
                //foreach (TrTextLine Line in TextLines)
                //{
                //    xRegion.Add(Line.ToXML());
                //    sb.Append(Line.TextEquiv);
                //    sb.Append(Environment.NewLine);
                //}
            }


            XElement xRegionText = new XElement(TrLibrary.xmlns + "TextEquiv",
                new XElement(TrLibrary.xmlns + "Unicode", sb.ToString()));
            xRegion.Add(xRegionText);

            // Debug.WriteLine(XRegion.ToString());
            return xRegion;
        }


        // CONSTRUCTORS ---------------------------------------------------------------------------------------

        // constructor ved indlæsning af XDoc
        public TrRegion_Table(string rType, string rID, string rTags, float rOrientation, string rCoords) : base(rType, rID, rTags, rOrientation, rCoords)
        {
            Cells.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // constructor ved skabelse af ny region
        public TrRegion_Table(int rOrder, float rOrientation, string rCoords) : base(rOrder, rOrientation, rCoords)
        {
            Cells.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // --------------------------------------------------------------------------------------------

        public void AddCell(TrCell Cell)
        {
            Cells.Add(Cell);
        }


    }
}
