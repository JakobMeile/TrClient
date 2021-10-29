// <copyright file="TrPage.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrPage.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TrPage : TrItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        /// <summary>
        /// Gets or sets the number of the page.
        /// </summary>
        public int Number { get; set; }

        public TrDocument ParentDocument { get; set; }

        public List<TrTranscript> Transcripts { get; set; }


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrPage"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parent">The page's parent: No item can be instantiated without a known parent.</param>
        public TrPage(string id, int pageNumber, string pageFileName, string imageFileURL, int width, int height, TrDocument parentDocument)
            : base(parentDocument)
        {
            ParentDocument = parentDocument;
            Transcripts = new List<TrTranscript>();

            IDNumber = id;
            Number = pageNumber;
            // ImageFileName = pageFileName;
            // ImageURL = imageFileURL;
            // Width = width;
            // Height = height;

            IsLoaded = false;
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
            var page = obj as TrPage;
            return Number.CompareTo(page.Number);
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 

        public List<TrRegion> Regions { get; set; }

        public List<TrTextLine> Lines
        {
            get
            {
                return Transcripts[0].Lines;
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

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 


    }
}
