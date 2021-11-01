// <copyright file="TrCollection.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrCollection.
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

    /// <summary>
    /// Item class for a collection.
    /// </summary>
    public class TrCollection : TrBase
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        public string Name { get; set; }

        public List<TrDocument> Documents { get; set; }

        public int DocumentCount { get; set; }

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrCollection"/> class.
        /// Default constructor.
        /// </summary>
        public TrCollection(string collectionName, string id, int documentCount)
        {
            Documents = new List<TrDocument>();
            
            Name = collectionName;
            IDNumber = id;
            DocumentCount = documentCount;

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
