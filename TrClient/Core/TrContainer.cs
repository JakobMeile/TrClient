// <copyright file="TrContainer.cs" company="PlaceholderCompany">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Media;

    public abstract class TrContainer : IEnumerable, INotifyPropertyChanged
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // enums                                                                                                              enums

        // ------------------------------------------------------------------------------------------------------------------------
        // abstract properties                                                                                  abstract properties

        // ------------------------------------------------------------------------------------------------------------------------
        // public properties                                                                                      public properties
        public TrItem ParentItem { get; set; }

        public int Count { get => itemList.Count; }

        public TrItem this[int index]
        {
            get { return itemList[index]; }
            set { itemList[index] = value; }
        }

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

                ParentItem.HasChanged = value;
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

                ParentItem.ChangesUploaded = value;
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
        protected List<TrItem> itemList;

        // ------------------------------------------------------------------------------------------------------------------------
        // private properties                                                                                    private properties

        // ------------------------------------------------------------------------------------------------------------------------
        // events                                                                                                            events
        public event PropertyChangedEventHandler PropertyChanged;

        // ------------------------------------------------------------------------------------------------------------------------
        // constructors                                                                                                constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrContainer"/> class.
        /// Default constructor.
        /// </summary>
        public TrContainer()
        {
            itemList = new List<TrItem>();
        }

        /// <summary>
        /// Non-default constructor.
        /// </summary>
        // ------------------------------------------------------------------------------------------------------------------------
        // interface-implementing methods                                                            interface-implementing methods
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)itemList).GetEnumerator();
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // abstract methods                                                                                        abstract methods

        // ------------------------------------------------------------------------------------------------------------------------
        // public override methods                                                                          public override methods

        /// <summary>
        /// Override af ToString().
        /// </summary>
        /// <returns>
        /// Ingenting (ikke implementeret).
        /// </returns>
        //public override string ToString()
        //{
        //    return "";
        //}
        // ------------------------------------------------------------------------------------------------------------------------
        // public methods                                                                                            public methods
        public bool Contains(TrItem item)
        {
            return itemList.Contains(item);
        }

        public void Add(TrItem item)
        {
            itemList.Add(item);
            item.ParentContainer = this;
            ParentItem.HasChanged = true;
        }

        public void Remove(TrItem item)
        {
            itemList.Remove(item);
            ParentItem.HasChanged = true;
        }

        public void RemoveAt(int i)
        {
            itemList.RemoveAt(i);
            ParentItem.HasChanged = true;
        }

        public void Sort()
        {
            itemList.Sort();
            ParentItem.HasChanged = true;
        }

        public void Reverse()
        {
            itemList.Reverse();
            ParentItem.HasChanged = true;
        }

        public void Clear()
        {
            itemList.Clear();
            ParentItem.HasChanged = true;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // private methods                                                                                          private methods
    }
}
