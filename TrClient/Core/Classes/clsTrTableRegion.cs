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
    public class clsTrTableRegion : clsTrRegion
    {
        public clsTrCells Cells = new clsTrCells();

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
                foreach (clsTrCell Cell in Cells)
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
                    foreach (clsTrCell Cell in Cells)
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
                    foreach (clsTrCell Cell in Cells)
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
            foreach (clsTrCell Cell in Cells)
            {
                foreach (clsTrTextLine TL in Cell.TextLines)        // OVERRIDE pga denne line
                    if (TL.HasStructuralTag)
                        TempList.Add(TL.StructuralTagValue);
            }

            List<string> TagList = TempList.Distinct().ToList();
            TagList.Sort();
            return TagList;
        }

        public override void Move(int Horizontally, int Vertically)
        {
            clsTrCoords C = new clsTrCoords(CoordsString);
            foreach (clsTrCoord CurrentCoord in C)
            {
                CurrentCoord.X = CurrentCoord.X + Horizontally;
                CurrentCoord.Y = CurrentCoord.Y + Vertically;
            }
            CoordsString = C.ToString();

            if (HasLines)
            {
                foreach (clsTrCell Cell in Cells)
                {
                    Cell.Move(Horizontally, Vertically);
                }

            }
        }

        public override bool DeleteShortBaselines(int Limit, clsTrLog Log)
        {
            bool RegionIsOK = true;
            bool CellIsOK = true;
            string ErrorMessage;

            foreach (clsTrCell Cell in Cells)
            {
                foreach (clsTrTextLine Line in Cell.TextLines)
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
            foreach (clsTrCell Cell in Cells)
            {
                foreach (clsTrTextLine Line in Cell.TextLines)
                    Line.SimplifyBoundingBox();
            }
        }

        public override void SimplifyBoundingBoxes(int MinimumHeight, int MaximumHeight)
        {
            foreach (clsTrCell Cell in Cells)
            {
                foreach (clsTrTextLine Line in Cell.TextLines)
                    Line.SimplifyBoundingBox(MinimumHeight, MaximumHeight);
            }
        }

        public override List<string> GetExpandedText(bool Refine, bool ConvertOtrema)
        {
            List<string> TempList = new List<string>();

            foreach (clsTrCell Cell in Cells)
            {
                foreach (clsTrTextLine TL in Cell.TextLines)
                    TempList.Add(TL.ExpandedText);
            }

            return TempList;
        }

        public override clsTrWords Words
        {
            get
            {
                foreach (clsTrCell Cell in Cells)
                {
                    foreach (clsTrTextLine TL in Cell.TextLines)
                    {
                        foreach (clsTrWord W in TL.Words)
                            _words.Add(W);
                    }
                }

                return _words;
            }
        }

        public override void ExtendBaseLines(clsTrDialogTransferSettings Settings, clsTrLog Log)
        {
            // Debug.WriteLine($"clsTrTextRegion : ExtendBaseLines");

            string ErrorMessage;

            if (HasLines)
            {
                foreach (clsTrCell Cell in Cells)
                {
                    foreach (clsTrTextLine Line in Cell.TextLines)
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

            XElement xRegion = new XElement(clsTrLibrary.xmlns + "TableRegion",
                new XAttribute("type", Type.ToString()),
                new XAttribute("orientation", Orientation.ToString()),
                new XAttribute("id", ID),
                new XAttribute("custom", CustomString),
                new XElement(clsTrLibrary.xmlns + "Coords",
                new XAttribute("points", CoordsString)));

            StringBuilder sb = new StringBuilder();

            foreach (clsTrCell Cell in Cells)
            {
                //foreach (clsTrTextLine Line in TextLines)
                //{
                //    xRegion.Add(Line.ToXML());
                //    sb.Append(Line.TextEquiv);
                //    sb.Append(Environment.NewLine);
                //}
            }


            XElement xRegionText = new XElement(clsTrLibrary.xmlns + "TextEquiv",
                new XElement(clsTrLibrary.xmlns + "Unicode", sb.ToString()));
            xRegion.Add(xRegionText);

            // Debug.WriteLine(XRegion.ToString());
            return xRegion;
        }


        // CONSTRUCTORS ---------------------------------------------------------------------------------------

        // constructor ved indlæsning af XDoc
        public clsTrTableRegion(string rType, string rID, string rTags, float rOrientation, string rCoords) : base(rType, rID, rTags, rOrientation, rCoords)
        {
            Cells.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // constructor ved skabelse af ny region
        public clsTrTableRegion(int rOrder, float rOrientation, string rCoords) : base(rOrder, rOrientation, rCoords)
        {
            Cells.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // --------------------------------------------------------------------------------------------

        public void AddCell(clsTrCell Cell)
        {
            Cells.Add(Cell);
        }


    }
}
