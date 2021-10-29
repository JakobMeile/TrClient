// <copyright file="TrTableRegion.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrTableRegion.
/// </summary>

namespace TrClient2.Core
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
        public List<TrCell> Cells { get; set; }

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrTableRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when loading a XML-document!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrTableRegion(string type, string id, string tags, float orientation, string coords, TrTranscript parentTranscript)
            : base(type, id, tags, orientation, coords, parentTranscript)
        {
            Cells = new List<TrCell>();

            // Tags.ParentRegion = this;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TrTableRegion"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used when creating a new document programmatically!
        /// </remarks>
        /// <param name="parent">The region's parent: No item can be instantiated without a known parent.</param>
        public TrTableRegion(int order, float orientation, string coords, TrTranscript parentTranscript)
            : base(order, orientation, coords, parentTranscript)
        {
            Cells = new List<TrCell>();

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


        public override List<TrTextLine> Lines
        {
            get
            {
                var selectedLines = Cells.SelectMany(x => x.TextLines).ToList();
                return selectedLines;
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
