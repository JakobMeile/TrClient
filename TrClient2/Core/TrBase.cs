// <copyright file="TrBase.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrBase.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media;

    /// <summary>
    /// Base class for all items (via <see cref="TrItem"/>).
    /// </summary>
    public abstract class TrBase : INotifyPropertyChanged
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        /// <summary>
        /// Holds the item's ID number (access via <see cref="ID"/>).
        /// </summary>
        private protected string idNumber;

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

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrBase"/> class.
        /// Default constructor.
        /// </summary>
        public TrBase()
        {
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
