// <copyright file="TrItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Beskrivelse ... / Hjælpeklasse for ...
/// </summary>

//
// Class KlasseNavn
//
// Arver:       ingen
// Base for:    ingen
//
// Versionshistorik m.v. - testet? fungerer? dato?
namespace TrClient.Core
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media;

    public abstract class TrItem : IComparable, INotifyPropertyChanged
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // enums                                                                                                              enums

        // ------------------------------------------------------------------------------------------------------------------------
        // abstract properties                                                                                  abstract properties
        public abstract TrItem Previous { get; }

        public abstract TrItem Next { get; }

        public abstract int LineCount { get; }

        // ------------------------------------------------------------------------------------------------------------------------
        // public properties                                                                                      public properties
        public TrContainer ParentContainer { get; set; }

        public string ID { get; set; }

        private bool isLoaded = false;

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

        private bool hasChanged = false;

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

        private bool changesUploaded = false;

        public bool ChangesUploaded
        {
            get
            {
                return changesUploaded;
            }

            set
            {
                changesUploaded = value;
                NotifyPropertyChanged("ChangesUploaded");
                if (changesUploaded)
                {
                    StatusColor = Brushes.DarkViolet;
                }

                ParentContainer.ChangesUploaded = value;
            }
        }

        private SolidColorBrush statusColor = Brushes.Red;

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

        // ------------------------------------------------------------------------------------------------------------------------
        // protected properties                                                                                protected properties

        // ------------------------------------------------------------------------------------------------------------------------
        // private properties                                                                                    private properties

        // ------------------------------------------------------------------------------------------------------------------------
        // events                                                                                                            events
        public event PropertyChangedEventHandler PropertyChanged;

        // ------------------------------------------------------------------------------------------------------------------------
        // constructors                                                                                                constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrItem"/> class.
        /// Default constructor.
        /// </summary>
        public TrItem()
        {
        }

        /// <summary>
        /// Non-default constructor.
        /// </summary>
        // ------------------------------------------------------------------------------------------------------------------------
        // interface-implementing methods                                                            interface-implementing methods
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // abstract methods                                                                                        abstract methods
        public abstract int CompareTo(object obj);

        // ------------------------------------------------------------------------------------------------------------------------
        // public override methods                                                                          public override methods

        /// <summary>
        /// Override af ToString()
        /// </summary>
        /// <returns>
        /// Ingenting (ikke implementeret)
        /// </returns>
        //public override string ToString()
        //{
        //    return "";
        //}

        // ------------------------------------------------------------------------------------------------------------------------
        // public methods                                                                                            public methods

        // ------------------------------------------------------------------------------------------------------------------------
        // private methods                                                                                          private methods
    }
}
