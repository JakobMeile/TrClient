// <copyright file="TrItem.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrItem.
/// </summary>

namespace TrClient.Core
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media;

    /// <summary>
    /// Base class for all items via <see cref="TrMainItem"/> or <see cref="TrPageLevelItem"/>.
    /// Inherits <see cref="TrBase"/>.
    /// </summary>
    public abstract class TrItem : TrBase, IComparable
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        /// <summary>
        /// Holds the item's parent container.
        /// </summary>
        private protected TrContainer parentContainer;

        /// <summary>
        /// Holds the item's ID number (access via <see cref="ID"/>).
        /// </summary>
        private protected string idNumber;

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrItem"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parentContainer">The item's parent container: No item can be instantiated without a known parent container.</param>
        public TrItem(TrContainer parentContainer)
        {
            ParentContainer = parentContainer;
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
        /// Gets or sets the item's parent container (of type TrContainer or derived).
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws exception if set to null.</exception>
        public TrContainer ParentContainer 
        { 
            get 
            {
                return parentContainer;
            }

            set
            {
                parentContainer = value;
                if (parentContainer == null)
                {
                    throw new ArgumentNullException("An item's parent container can't be null.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the item's ID number (as used by Transkribus).
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws exception if set to null.</exception>
        /// <exception cref="ArgumentException">Throws exception if set to empty string.</exception>
        public string IDNumber
        {
            get
            {
                return idNumber;
            }

            set
            {
                idNumber = value;
                if (idNumber == null)
                {
                    throw new ArgumentNullException("An item's ID can't be null.");
                }
                else if (idNumber == string.Empty)
                {
                    throw new ArgumentException("An item's ID can't be an empty string.");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item has changed since it was loaded (saved).
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return hasChanged;
            }

            set
            {
                hasChanged = value;
                NotifyPropertyChanged("HasChanged");
                if (hasChanged)
                {
                    StatusColor = Brushes.Orange;
                }

                ParentContainer.HasChanged = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether any changes to the item has has been uploaded.
        /// </summary>
        public bool IsChangesUploaded
        {
            get
            {
                return isChangesUploaded;
            }

            set
            {
                isChangesUploaded = value;
                NotifyPropertyChanged("IsChangesUploaded");
                if (isChangesUploaded)
                {
                    StatusColor = Brushes.DarkViolet;
                }

                ParentContainer.IsChangesUploaded = value;
            }
        }

        /// <summary>
        /// Gets the number of lines of the item; from Collection-wide (returns many) down to single TrTextLine (returns 1).
        /// When the item is below TrTextLine-level, LineCount returns 0.
        /// </summary>
        public abstract int LineCount { get; }

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
