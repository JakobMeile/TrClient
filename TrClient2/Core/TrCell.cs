// <copyright file="TrCell.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrCell.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public class TrCell : TrPageLevelItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        private TrTableRegion _parentRegion;

        private List<TrTextLine> _textLines;

        private int _rowNumber;

        private int _columnNumber;

        private string _cornerPoints;


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrCell"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parent">The line's parent: No item can be instantiated without a known parent.</param>
        public TrCell(TrTableRegion parentRegion, string id, string row, string column, string coords, string cornerPoints)
        {
            IDNumber = id;
            RowNumber = Convert.ToInt32(row);
            ColumnNumber = Convert.ToInt32(column);
            CoordinatesString = coords;
            CornerPoints = cornerPoints;

            ParentRegion = parentRegion;
            TextLines = new List<TrTextLine>();
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 4. Finalizers 

        // ------------------------------------------------------------------------------------------------------------------------
        // 5. Delegates 

        // ------------------------------------------------------------------------------------------------------------------------
        // 6. Events 

        // ------------------------------------------------------------------------------------------------------------------------
        // 7. Enums 

        // ------------------------------------------------------------------------------------------------------------------------
        // 8. Interface implementations 

        /// <summary>
        /// Implementation regarding IComparable: Compares this item with another item of the same kind.
        /// </summary>
        /// <param name="obj">The other item to be compared with.</param>
        /// <returns>An integer with value.... ??????</returns>
        public override int CompareTo(object obj)
        {
            var cell = obj as TrCell;
            return Number.CompareTo(cell.Number);
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 
        public TrTableRegion ParentRegion { get; set; }
        public List<TrTextLine> TextLines { get; set; }

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        public string CornerPoints { get; set; }

        public override int PageNumber
        {
            get
            {
                return ParentRegion.PageNumber;
            }
        }

        /// <summary>
        /// Gets or sets the number of the cell.
        /// </summary>
        public override int Number
        {
            get
            {
                // temporary code begins
                // _number = ParentRegion.Cells.IndexOf(this);
                // temporary code ends

                return _number;
            }

            set
            {
                _number = value;
            }
        }


        /// <summary>
        /// Gets the previous item of its kind.
        /// </summary>
        public override TrItem Previous
        {
            get;
        }

        /// <summary>
        /// Gets the next item of its kind.
        /// </summary>
        public override TrItem Next
        {
            get;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 10. Indexers 

        // ------------------------------------------------------------------------------------------------------------------------
        // 11. Methods 

        public List<TrTextLine> GetLines()
        {
            return _textLines;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 


    }
}
