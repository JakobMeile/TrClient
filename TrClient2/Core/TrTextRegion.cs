// <copyright file="TrTextRegion.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrTextRegion.
/// </summary>

namespace TranskribusClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TrTextRegion : TrRegion
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        private List<TrTextLine> _textLines;


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrTextRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when loading a XML-document!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrTextRegion(TrTranscript parentTranscript, string id, string type, int order, string tags, float orientation, string coords)
            : base(parentTranscript, id, type, order, orientation, coords)
        {
            // order??
            _textLines = new List<TrTextLine>();

            // Tags.ParentRegion = this;
        }

        // constructor ved skabelse af ny region
        /// <summary>
        /// Initializes a new instance of the <see cref="TrTextRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when creating a new region programmatically!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrTextRegion(TrTranscript parentTranscript, string id, string type, int order, float orientation, string coords)
            : base(parentTranscript, id, type, order, orientation, coords)
        {
            _textLines = new List<TrTextLine>();

            // Tags.ParentRegion = this;
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
        public override List<TrTextLine> GetLines()
        {
            return _textLines;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 

    }
}
