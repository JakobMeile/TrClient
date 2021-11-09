// <copyright file="TrTranscript.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrTranscript.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for transcript.
    /// Inherits <see cref="TrPageLevelItem"/>.
    /// </summary>
    public class TrTranscript : TrPageLevelItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        private TrPage _parentPage;

        private List<TrRegion> _regions;
        


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrTranscript"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parent">The transcripts's parent: No item can be instantiated without a known parent.</param>
        public TrTranscript(TrPage parentPage, string id, string key, int pageNumber, string status, string user, string timeStamp)
        {
            ParentPage = parentPage;
            _regions = new List<TrRegion>();

            // TranscriptionDocument = new XDocument();

            IDNumber = id;
            // Key = key;
            // PageNumber = pageNumber; // SNOT
            // Status = status;
            // User = user;
            // TimeStamp = timeStamp;

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
            var transcript = obj as TrTranscript;
            return Number.CompareTo(transcript.Number);
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 
        public TrPage ParentPage
        {
            get { return _parentPage; }
            set { _parentPage = value; }
        }




        public override int PageNumber
        {
            get { return _pageNumber; }
            // set { _pageNumber = value; }
        }




        /// <summary>
        /// Gets or sets the number of the transcript.
        /// </summary>

        public override int Number
        {
            get
            {
                // temporary code begins
                // _number = ParentPage.Transcripts.IndexOf(this);
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
            var selectedLines = _regions.SelectMany(x => x.GetLines()).ToList();
            return selectedLines;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 



    }
}
