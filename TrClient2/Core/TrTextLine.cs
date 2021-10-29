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

        public TrTextRegion ParentRegion { get; set; }

        public TrCell ParentCell { get; set; }

        public bool IsInTable
        {
            get
            {
                return (ParentRegion == null && ParentCell != null);
            }
        }

        private string baseLineCoordinatesString;

        // temporary code begins
        public string Content { get; set; }
        // temporary code ends

        private int regionNumber;

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
        public TrTextLine(string id, string tags, string lineCoords, string baseLineCoords, string textEquiv, TrTextRegion parentRegion)
            : base(parentRegion)
        {
            IDNumber = id;
            // TagString = tags;
            // ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordinatesString = lineCoords;
            BaseLineCoordinatesString = baseLineCoords;
            Content = textEquiv;

            ParentRegion = parentRegion;
            ParentCell = null;
            pageNumber = ParentRegion.PageNumber;
            regionNumber = parentRegion.Number;

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
        public TrTextLine(string id, string tags, string lineCoords, string baseLineCoords, string textEquiv, TrCell parentCell)
            : base(parentCell)
        {
            IDNumber = id;
            // TagString = tags;
            // ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordinatesString = lineCoords;
            BaseLineCoordinatesString = baseLineCoords;
            Content = textEquiv;

            ParentRegion = null;
            ParentCell = parentCell;
            pageNumber = ParentCell.PageNumber;
            regionNumber = ParentCell.ParentRegion.Number;

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
        public override int PageNumber
        {
            get
            {
                return pageNumber;
            }
        }

        public int RegionNumber
        {
            get
            {
                return regionNumber;
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
                number = ParentRegion.Lines.IndexOf(this);
                // temporary code ends

                return number;
            }

            set
            {
                number = value;
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
        /// <summary>
        /// Gets or sets the string with baseline coordinates.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws exception if set to null.</exception>
        /// <exception cref="ArgumentException">Throws exception if set to empty string.</exception>
        public string BaseLineCoordinatesString
        {
            get
            {
                return baseLineCoordinatesString;
            }

            set
            {
                baseLineCoordinatesString = value;
                if (baseLineCoordinatesString == null)
                {
                    throw new ArgumentNullException("A coordinate string can't be null.");
                }
                else if (baseLineCoordinatesString == string.Empty)
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
