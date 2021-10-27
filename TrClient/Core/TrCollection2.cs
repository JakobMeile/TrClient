// <copyright file="TrCollection2.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrCollection2.
/// </summary>

namespace TrClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Item class for a collection.
    /// Inherits <see cref="TrItem"/>.
    /// </summary>
    public class TrCollection2 : TrItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        private string name;

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrCollection2"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parentContainer">The collection's parent container: No item can be instantiated without a known parent container.</param>
        public TrCollection2(TrContainer parentContainer)
            : base(parentContainer)
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
        public override int CompareTo(object obj)
        {
            var collection = obj as TrCollection2;
            return Name.CompareTo(collection.Name);
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 
        
        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        public string Name
        { 
            get 
            {
                return name;
            }
            
            set 
            {
                name = value;
            }
        }

        /// <summary>
        /// Gets the number of lines of the item; from Collection-wide (returns many) down to single TrTextLine (returns 1).
        /// When the item is below TrTextLine-level, LineCount returns 0.
        /// </summary>
        public override int LineCount 
        { 
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the previous item of its kind.
        /// </summary>
        public override TrItem Previous
        {
            get
            {
                TrCollection2 test = new TrCollection2(ParentContainer);
                return test;
            }
        }


        /// <summary>
        /// Gets the next item of its kind.
        /// </summary>
        public override TrItem Next
        {
            get
            {
                TrCollection2 test = new TrCollection2(ParentContainer);
                return test;
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
