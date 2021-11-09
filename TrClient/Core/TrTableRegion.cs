// <copyright file="TrTableRegion.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;
    using System.Xml.Linq;
    using TranskribusClient.Extensions;
    using TranskribusClient.Helpers;
    using TranskribusClient.Libraries;

    public class TrTableRegion : TrRegion
    {
        public TrCells Cells = new TrCells();

        // OVERRIDE PROPERTIES ----------------------------------------------------------------------
        public override bool ChangesUploaded
        {
            get
            {
                return changesUploaded;
            }

            set
            {
                changesUploaded = value;
                if (changesUploaded)
                {
                    StatusColor = Brushes.DarkViolet;
                }

                NotifyPropertyChanged("ChangesUploaded");
                foreach (TrCell cell in Cells)
                {
                    cell.TextLines.ChangesUploaded = value;          // OVERRIDE pga denne linie
                }
            }
        }

        public override bool HasLines
        {
            get
            {
                bool temp = false;
                if (Cells.Count > 0)
                {
                    foreach (TrCell cell in Cells)
                    {
                        temp = temp || (cell.TextLines.Count > 0);
                    }
                }

                return temp;
            }
        }

        private int numberOfLines = 0;

        public override int NumberOfLines
        {
            get
            {
                int temp = 0;
                if (Cells.Count > 0)
                {
                    foreach (TrCell cell in Cells)
                    {
                        temp = temp + cell.TextLines.Count;
                    }
                }

                numberOfLines = temp;
                return numberOfLines;
            }
        }

        // OVERRIDE METHODS ------------------------------------------------------------------------------------
        public override List<string> GetStructuralTags()
        {
            List<string> tempList = new List<string>();
            foreach (TrCell cell in Cells)
            {
                foreach (TrTextLine textLine in cell.TextLines)        // OVERRIDE pga denne line
                {
                    if (textLine.HasStructuralTag)
                    {
                        tempList.Add(textLine.StructuralTagValue);
                    }
                }
            }

            List<string> tagList = tempList.Distinct().ToList();
            tagList.Sort();
            return tagList;
        }

        public override void Move(int horizontally, int vertically)
        {
            TrCoords c = new TrCoords(CoordsString);
            foreach (TrCoord currentCoord in c)
            {
                currentCoord.X = currentCoord.X + horizontally;
                currentCoord.Y = currentCoord.Y + vertically;
            }

            CoordsString = c.ToString();

            if (HasLines)
            {
                foreach (TrCell cell in Cells)
                {
                    cell.Move(horizontally, vertically);
                }
            }
        }

        public override bool DeleteShortBaselines(int limit, TrLog log)
        {
            bool regionIsOK = true;
            bool cellIsOK = true;
            string errorMessage;

            foreach (TrCell cell in Cells)
            {
                foreach (TrTextLine line in cell.TextLines)
                {
                    if (line.Width < limit)
                    {
                        cellIsOK = false;
                        line.MarkToDeletion = true;
                        errorMessage = $"Width = {line.Width}: Line deleted!";
                        log.Add(line, errorMessage);
                    }
                }

                if (!cellIsOK)
                {
                    for (int i = cell.TextLines.Count - 1; i >= 0; i--)
                    {
                        if (cell.TextLines[i].MarkToDeletion)
                        {
                            cell.TextLines.RemoveAt(i);
                        }
                    }
                }

                regionIsOK = regionIsOK && cellIsOK;
            }

            return regionIsOK;
        }

        public override void SimplifyBoundingBoxes()
        {
            foreach (TrCell cell in Cells)
            {
                foreach (TrTextLine line in cell.TextLines)
                {
                    line.SimplifyBoundingBox();
                }
            }
        }

        public override void SimplifyBoundingBoxes(int minimumHeight, int maximumHeight)
        {
            foreach (TrCell cell in Cells)
            {
                foreach (TrTextLine line in cell.TextLines)
                {
                    line.SimplifyBoundingBox(minimumHeight, maximumHeight);
                }
            }
        }

        public override List<string> GetExpandedText(bool refine, bool convertOtrema)
        {
            List<string> tempList = new List<string>();

            foreach (TrCell cell in Cells)
            {
                foreach (TrTextLine textLine in cell.TextLines)
                {
                    tempList.Add(textLine.ExpandedText);
                }
            }

            return tempList;
        }

        public override TrWords Words
        {
            get
            {
                foreach (TrCell cell in Cells)
                {
                    foreach (TrTextLine textLine in cell.TextLines)
                    {
                        foreach (TrWord w in textLine.Words)
                        {
                            words.Add(w);
                        }
                    }
                }

                return words;
            }
        }

        public override void ExtendBaseLines(TrDialogTransferSettings settings, TrLog log)
        {
            // Debug.WriteLine($"TrRegion_Text : ExtendBaseLines");
            string errorMessage;

            if (HasLines)
            {
                foreach (TrCell cell in Cells)
                {
                    foreach (TrTextLine line in cell.TextLines)
                    {
                        if (settings.ExtendLeft)
                        {
                            line.ExtendLeft(settings.LeftAmount);
                            errorMessage = $"Line extended left with {settings.LeftAmount}!";
                            log.Add(line, errorMessage);
                        }

                        if (settings.ExtendRight)
                        {
                            line.ExtendRight(settings.RightAmount);
                            errorMessage = $"Line extended right with {settings.RightAmount}!";
                            log.Add(line, errorMessage);
                        }
                    }
                }
            }
        }

        public override void ConvertAsterisksToHyphenation()
        {
            foreach (TrCell cell in Cells)
            {
                foreach (TrTextLine textLine in cell.TextLines)
                    textLine.ConvertAsterisksToHyphenation();
            }
        }

        public override XElement ToXML()
        {
            string customString = Tags.ToString();

            XElement xRegion = new XElement(
                TrLibrary.Xmlns + "TableRegion",
                new XAttribute("type", Type.ToString()),
                new XAttribute("orientation", Orientation.ToString()),
                new XAttribute("id", ID),
                new XAttribute("custom", customString),
                new XElement(
                    TrLibrary.Xmlns + "Coords",
                    new XAttribute("points", CoordsString)));

            StringBuilder sb = new StringBuilder();

            foreach (TrCell cell in Cells)
            {
                //foreach (TrTextLine Line in TextLines)
                //{
                //    xRegion.Add(Line.ToXML());
                //    sb.Append(Line.TextEquiv);
                //    sb.Append(Environment.NewLine);
                //}
            }

            XElement xRegionText = new XElement(
                TrLibrary.Xmlns + "TextEquiv",
                new XElement(TrLibrary.Xmlns + "Unicode", sb.ToString()));
            xRegion.Add(xRegionText);

            // Debug.WriteLine(XRegion.ToString());
            return xRegion;
        }

        // CONSTRUCTORS ---------------------------------------------------------------------------------------

        // constructor ved indlæsning af XDoc
        public TrTableRegion(string rType, string rID, string rTags, float rOrientation, string rCoords, TrRegions parentContainer)
            : base(rType, rID, rTags, rOrientation, rCoords, parentContainer)
        {
            Cells.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // constructor ved skabelse af ny region
        public TrTableRegion(int rOrder, float rOrientation, string rCoords, TrRegions parentContainer)
            : base(rOrder, rOrientation, rCoords, parentContainer)
        {
            ParentContainer = parentContainer;
            ParentTranscript = parentContainer.ParentTranscript;
            Cells.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // --------------------------------------------------------------------------------------------
        public void AddCell(TrCell cell)
        {
            Cells.Add(cell);
        }
    }
}
