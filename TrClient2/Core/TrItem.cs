// <copyright file="TrItem.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrItem.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Windows.Media;

    /// <summary>
    /// Base class for all items, directly or via <see cref="TrPageLevelItem"/>.
    /// </summary>
    public abstract class TrItem : IComparable, INotifyPropertyChanged
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        /// <summary>
        /// Holds a color value indicating the status of the item.
        /// </summary>
        private protected SolidColorBrush statusColor = Brushes.Red;

        /// <summary>
        /// Holds a boolean flag indicating whether the item is loaded from the server.
        /// </summary>
        private protected bool isLoaded = false;

        /// <summary>
        /// Holds a boolean flag indicating whether the item has changed since it was loaded (saved).
        /// </summary>
        private protected bool hasChanged = false;

        /// <summary>
        /// Holds a boolean flag indicating whether any changes to the item has has been uploaded.
        /// </summary>
        private protected bool isChangesUploaded = false;

        private protected TrItem parent;

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
        private protected TrItem(TrItem parentItem)
        {
            parent = parentItem;
        }
        private protected TrItem()
        { }

        // public TrItem() { }

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
        /// Implementation regarding IComparable: Compares this item with another item of the same kind.
        /// </summary>
        /// <param name="obj">The other item to be compared with.</param>
        /// <returns>An integer with value.... ??????</returns>
        public abstract int CompareTo(object obj);

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

        public TrItem Parent
        {
            get
            {
                return parent;
            }
            
            set 
            {
                if (parent != value)
                {
                    parent = value;
                    NotifyPropertyChanged("Parent");
                }
            }
        }

        /// <summary>
        /// Gets or sets a color value indicating the status of the item.
        /// </summary>
        public SolidColorBrush StatusColor
        {
            get
            {
                return statusColor;
            }

            set
            {
                if (statusColor != value)
                {
                    statusColor = value;
                    NotifyPropertyChanged("StatusColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is loaded from the server.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }

            set
            {
                if (isLoaded != value)
                {
                    isLoaded = value;
                    NotifyPropertyChanged("IsLoaded");
                    switch (isLoaded)
                    {
                        case true:
                            StatusColor = Brushes.LimeGreen;
                            break;
                        case false:
                            StatusColor = Brushes.Red;
                            break;
                    }
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

                Parent.HasChanged = value;
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

                Parent.IsChangesUploaded = value;
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
