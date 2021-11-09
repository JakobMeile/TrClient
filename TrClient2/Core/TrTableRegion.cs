// <copyright file="TrTableRegion.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrTableRegion.
/// </summary>

namespace TranskribusClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TrTableRegion : TrRegion
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        private List<TrCell> _cells;


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrTableRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when loading a XML-document!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrTableRegion(TrTranscript parentTranscript, string id, string type, string tags, float orientation, string coords)
            : base(parentTranscript, id, type, tags, orientation, coords)
        {
            _cells = new List<TrCell>();

            // Tags.ParentRegion = this;
        }
        // public TrRegion(string rType, string rID, string rTags, float rOrientation, string rCoords, TrRegions parentContainer)


        /// <summary>
        /// Initializes a new instance of the <see cref="TrTableRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when creating a new document programmatically!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrTableRegion(TrTranscript parentTranscript, string id, string type, int order, float orientation, string coords)
            : base(parentTranscript, id, type, order, orientation, coords)
        {
            _cells = new List<TrCell>();

            // Tags.ParentRegion = this;
        }
        //         public TrRegion(int rOrder, float rOrientation, string rCoords, TrRegions parentContainer)


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
        public override List<TrTextLine> GetLines()
        {
            var selectedLines = _cells.SelectMany(x => x.GetLines()).ToList();
            return selectedLines;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 

    }
}
