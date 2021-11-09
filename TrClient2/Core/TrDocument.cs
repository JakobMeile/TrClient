// <copyright file="TrDocument.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrDocument.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TrDocument : TrBase, IComparable
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        /// <summary>
        /// Holds the title of the document.
        /// </summary>
        private string _title = string.Empty;

        private TrCollection _parentCollection;

        private List<TrPage> _pages;

        private int _pageCount;


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrDocument"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parent">The document's parent: No item can be instantiated without a known parent.</param>
        public TrDocument(TrCollection parentCollection, string id, string documentTitle, int pageCount)
        {
            ParentCollection = parentCollection;
            _pages = new List<TrPage>();

            Title = documentTitle;
            IDNumber = id;
            PageCount = pageCount;

            Status = TrEnum.Status.Unloaded;
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
        public int CompareTo(object obj)
        {
            var document = obj as TrDocument;
            return Title.CompareTo(document.Title);
        }


        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 

        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        public string Title 
        { 
            get { return _title; } 
            set { _title = value; } 
        }

        public TrCollection ParentCollection
        {
            get { return _parentCollection; }
            set { _parentCollection = value; }
        }


        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = value; }
        }





        // ------------------------------------------------------------------------------------------------------------------------
        // 10. Indexers 

        // ------------------------------------------------------------------------------------------------------------------------
        // 11. Methods 
        public List<TrTextLine> GetLines()
        {
            var selectedLines = _pages.SelectMany(x => x.GetLines()).ToList();
            return selectedLines;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 


    }
}
