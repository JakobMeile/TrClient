// <copyright file="TrItem.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrItem.
/// </summary>

namespace TranskribusClient2.Core
{
    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Windows.Media;

    /// <summary>
    /// Base class for all items, directly or via <see cref="TrPageLevelItem"/>.
    /// </summary>
    public abstract class TrItem : TrBase, IComparable
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrItem"/> class.
        /// Default constructor.
        /// </summary>
        private protected TrItem()
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

        /// <summary>
        /// Implementation regarding IComparable: Compares this item with another item of the same kind.
        /// </summary>
        /// <param name="obj">The other item to be compared with.</param>
        /// <returns>An integer with value.... ??????</returns>
        public abstract int CompareTo(object obj);


        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 

        /// <summary>
        /// Gets the previous item of its kind.
        /// </summary>
        public abstract TrItem Previous { get; }

        /// <summary>
        /// Gets the next item of its kind.
        /// </summary>
        public abstract TrItem Next { get; }

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
