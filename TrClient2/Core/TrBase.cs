// <copyright file="TrBase.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrBase.
/// </summary>

namespace TranskribusClient2.Core
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
        private protected string _idNumber;

        /// <summary>
        /// Holds a value indicating the status of the item.
        /// </summary>
        private protected TrEnum.Status _status = TrEnum.Status.Unloaded;

        /// <summary>
        /// Holds a color value indicating the status of the item.
        /// </summary>
        private protected SolidColorBrush _statusColor = Brushes.Red;

        /// <summary>
        /// Holds a boolean flag indicating whether the item is loaded from the server.
        /// </summary>
        private protected bool _isLoaded = false;

        /// <summary>
        /// Holds a boolean flag indicating whether the item has changed since it was loaded (saved).
        /// </summary>
        private protected bool _hasChanged = false;

        /// <summary>
        /// Holds a boolean flag indicating whether any changes to the item has has been uploaded.
        /// </summary>
        private protected bool _isChangesUploaded = false;


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
                return _idNumber;
            }

            set
            {
                _idNumber = value;
                if (_idNumber == null)
                {
                    throw new ArgumentNullException("An item's ID can't be null.");
                }
                else if (_idNumber == string.Empty)
                {
                    throw new ArgumentException("An item's ID can't be an empty string.");
                }
            }
        }

        public TrEnum.Status Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status != value)
                {
                    if (value >= TrEnum.Status.Unloaded && value <= TrEnum.Status.Refreshed)
                    {
                        _status = value;
                        NotifyPropertyChanged("Status");
                        SetStatusColor();
                    }
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
                return _statusColor;
            }

            set
            {
                if (_statusColor != value)
                {
                    _statusColor = value;
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
                return _isLoaded;
            }

            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyPropertyChanged("IsLoaded");
                    switch (_isLoaded)
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

        public bool HasChanged
        {
            get
            {
                return _hasChanged;
            }

            set
            {
                _hasChanged = value;
                NotifyPropertyChanged("HasChanged");
                if (_hasChanged)
                {
                    StatusColor = Brushes.Orange;
                }

            }
        }

        public bool IsChangesUploaded
        {
            get
            {
                return _isChangesUploaded;
            }

            set
            {
                _isChangesUploaded = value;
                NotifyPropertyChanged("IsChangesUploaded");
                if (_isChangesUploaded)
                {
                    StatusColor = Brushes.DarkViolet;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 10. Indexers 

        // ------------------------------------------------------------------------------------------------------------------------
        // 11. Methods 

        private protected void SetStatusColor()
        {
            switch (Status)
            {
                case TrEnum.Status.Unloaded:
                    StatusColor = Brushes.Red; 
                    break;
                case TrEnum.Status.Loaded:
                    StatusColor = Brushes.LimeGreen; 
                    break;
                case TrEnum.Status.Changed:
                    StatusColor = Brushes.Orange; 
                    break;
                case TrEnum.Status.Uploaded:
                    StatusColor = Brushes.DarkViolet; 
                    break;
                case TrEnum.Status.Refreshed:
                    StatusColor = Brushes.Cyan;
                    break;
                default:
                    StatusColor = Brushes.Red;
                    break;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 
        
    
    }
}
