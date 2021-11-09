// <copyright file="TrTextLine.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrTextLine.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    // using TrClient.Core.Tags;

    public class TrTextLine : TrPageLevelItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        // public ObservableCollection<TrTag> Tags { get; set; }
        private TrTextRegion _parentRegion;

        private TrCell _parentCell;

        private bool _isInTable;


        private string _baseLineCoordinatesString;

        private string _content;

        private int _regionNumber;

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrTextLine"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used for normal textlines (not in table cells!).
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <param name="lineCoords"></param>
        /// <param name="baseLineCoords"></param>
        /// <param name="textEquiv"></param>
        /// <param name="parentRegion"></param>
        public TrTextLine(TrTextRegion parentRegion, string id, string tags, string lineCoords, string baseLineCoords, string textEquiv)
        {
            IDNumber = id;
            // TagString = tags;
            // ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordinatesString = lineCoords;
            BaseLineCoordinatesString = baseLineCoords;
            Content = textEquiv;

            ParentRegion = parentRegion;
            ParentCell = null;
            _pageNumber = ParentRegion.PageNumber;
            _regionNumber = parentRegion.Number;

            //Tags.ParentRegion = ParentRegion;
            //Tags.ParentLine = this;

            //Tags.LoadFromCustomAttribute(tTags);

            //if (Tags.Count > 0)
            //{
            //    foreach (TrTag tag in Tags)
            //    {
            //        if (tag.GetType() == typeof(TrTagStructural))
            //        {
            //            StructuralTag = (TrTagStructural)tag;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrTextLine"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used for textlines in table cells!
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <param name="lineCoords"></param>
        /// <param name="baseLineCoords"></param>
        /// <param name="textEquiv"></param>
        /// <param name="parentCell"></param>
        public TrTextLine(TrCell parentCell, string id, string tags, string lineCoords, string baseLineCoords, string textEquiv)
        {
            IDNumber = id;
            // TagString = tags;
            // ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordinatesString = lineCoords;
            BaseLineCoordinatesString = baseLineCoords;
            Content = textEquiv;

            ParentRegion = null;
            ParentCell = parentCell;
            _pageNumber = ParentCell.PageNumber;
            _regionNumber = ParentCell.ParentRegion.Number;

            //Tags.ParentRegion = ParentRegion;
            //Tags.ParentLine = this;

            //Tags.LoadFromCustomAttribute(tTags);

            //if (Tags.Count > 0)
            //{
            //    foreach (TrTag tag in Tags)
            //    {
            //        if (tag.GetType() == typeof(TrTagStructural))
            //        {
            //            StructuralTag = (TrTagStructural)tag;
            //        }
            //    }
            //}
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
            var line = obj as TrTextLine;
            return Number.CompareTo(line.Number);
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 
        public TrTextRegion ParentRegion
        {
            get { return _parentRegion; }
            set { _parentRegion = value; }
        }


        public TrCell ParentCell
        {
            get { return _parentCell; }
            set { _parentCell = value; }
        }


        public bool IsInTable
        {
            get
            {
                return (ParentRegion == null && ParentCell != null);
            }
        }
        public override int PageNumber
        {
            get
            {
                return _pageNumber;
            }
        }

        public int RegionNumber
        {
            get
            {
                return _regionNumber;
            }
        }

        /// <summary>
        /// Gets or sets the number of the line.
        /// </summary>
        public override int Number
        {
            get
            {
                // temporary code begins
                // _number = ParentRegion.Lines.IndexOf(this);
                // temporary code ends

                return _number;
            }

            set
            {
                _number = value;
            }
        }

        // temporary code begins
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        // temporary code ends

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
        /// <summary>
        /// Gets or sets the string with baseline coordinates.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws exception if set to null.</exception>
        /// <exception cref="ArgumentException">Throws exception if set to empty string.</exception>
        public string BaseLineCoordinatesString
        {
            get
            {
                return _baseLineCoordinatesString;
            }

            set
            {
                _baseLineCoordinatesString = value;
                if (_baseLineCoordinatesString == null)
                {
                    throw new ArgumentNullException("A coordinate string can't be null.");
                }
                else if (_baseLineCoordinatesString == string.Empty)
                {
                    throw new ArgumentException("A coordinate string can't be empty.");
                }
            }
        }



        // ------------------------------------------------------------------------------------------------------------------------
        // 10. Indexers 

        // ------------------------------------------------------------------------------------------------------------------------
        // 11. Methods 

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 


    }
}
