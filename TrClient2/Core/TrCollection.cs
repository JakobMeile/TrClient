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
    using System.Diagnostics;
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
        /// Holds the name of the collection.
        /// </summary>
        private string _name = string.Empty;

        private List<TrDocument> _documents;

        private int _documentCount;


        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrCollection"/> class.
        /// Default constructor.
        /// </summary>
        public TrCollection(string collectionName, string id, int documentCount)
        {
            _documents = new List<TrDocument>();
            
            Name = collectionName;
            IDNumber = id;
            DocumentCount = documentCount;

            this.PropertyChanged += TrDocument_PropertyChanged;

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
        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        public string Name 
        { 
            get { return _name;  } 
            set { _name = value; }
        }


        public int DocumentCount
        {
            get { return _documentCount; }
            set { _documentCount = value; }
        }


        // ------------------------------------------------------------------------------------------------------------------------
        // 10. Indexers 

        // ------------------------------------------------------------------------------------------------------------------------
        // 11. Methods 
        void TrDocument_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.Print($"TrDocument_PropertyChanged:");
            switch (e.PropertyName)
            {
                case "Status": 
                    this.Status = (sender as TrDocument).Status;
                    Debug.Print($"Status changed for document: {(sender as TrDocument).Title}: Status = {(sender as TrDocument).Status}");
                    break;
                default:
                    break;

            }
        }

      

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 

    }
}
