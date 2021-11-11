// <copyright file="TrDataContainer.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrDataContainer.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TrDataContainer
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        public ObservableCollection<TrTextLine> Lines { get; set; }

        public Dictionary<string, TrDocument> DocumentsDictionary = new Dictionary<string, TrDocument>();
        public Dictionary<string, TrPage> PagesDictionary = new Dictionary<string, TrPage>();
        public Dictionary<string, TrTranscript> TranscriptsDictionary = new Dictionary<string, TrTranscript>();
        public Dictionary<string, TrRegion> RegionsDictionary = new Dictionary<string, TrRegion>();
        public Dictionary<string, TrCell> CellsDictionary = new Dictionary<string, TrCell>();
        public Dictionary<string, TrTextLine> TextLinesDictionary = new Dictionary<string, TrTextLine>();

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrDataContainer"/> class.
        /// Default constructor.
        /// </summary>
        public TrDataContainer()
        {
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

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 

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
