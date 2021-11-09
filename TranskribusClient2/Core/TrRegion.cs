// <copyright file="TrRegion.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrRegion.
/// </summary>

namespace TranskribusClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for TrTextRegion and TrTableRegion.
    /// Inherits <see cref="TrPageLevelItem"/>.
    /// </summary>
    public abstract class TrRegion : TrPageLevelItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        private TrTranscript _parentTranscript;

        

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when loading a XML-document!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrRegion(TrTranscript parentTranscript, string id, string type, string tags, float orientation, string coords)
        {
            // Type = type;
            IDNumber = id;
            // TagString = rTags;
            // ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordinatesString = coords;
            // Orientation = rOrientation;

            ParentTranscript = parentTranscript;

            //Tags.LoadFromCustomAttribute(rTags);
            
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
        /// Initializes a new instance of the <see cref="TrRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when creating a new region programmatically!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrRegion(TrTranscript parentTranscript, string id, string type, int order, float orientation, string coords)
        {
            // Type = string.Empty;
            // IDNumber = "region_" + TrLibrary.GetNewTimeStamp().ToString();
            // ReadingOrder = order;
            CoordinatesString = coords;
            // Orientation = orientation;

            ParentTranscript = parentTranscript;
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
            var region = obj as TrRegion;
            return Number.CompareTo(region.Number);
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 

        public TrTranscript ParentTranscript
        {
            get { return _parentTranscript; }
            set { _parentTranscript = value; }
        }


        public override int PageNumber 
        {
            get 
            {
                return ParentTranscript.PageNumber; 
            }
        }

        /// <summary>
        /// Gets or sets the number of the region.
        /// </summary>
        public override int Number
        {
            get
            {
                // temporary code begins
                // _number = ParentTranscript.Regions.IndexOf(this);
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
        public abstract List<TrTextLine> GetLines();

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 





    }
}
