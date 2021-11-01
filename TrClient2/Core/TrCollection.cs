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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Item class for a collection.
    /// </summary>
    public class TrCollection : INotifyPropertyChanged
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

        /// <summary>
        /// Raises when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // ------------------------------------------------------------------------------------------------------------------------
        // 7. Enums 

        // ------------------------------------------------------------------------------------------------------------------------
        // 8. Interface implementations 
     
        /// <summary>
        /// Implementation regarding INotifyPropertyChanged
        /// Raises a new event, telling that the property in question has changed.
        /// </summary>
        /// <param name="propName">The name of the property that has changed.</param>
        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

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
