// <copyright file="TrContainer.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrContainer.
/// </summary>

namespace TrClient.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Media;

    /// <summary>
    /// Base class for all collections of items. 
    /// Inherits <see cref="TrBase"/>.
    /// </summary>
    public abstract class TrContainer : TrBase, IEnumerable
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        /// <summary>
        /// Holds the container's parent item.
        /// </summary>
        private protected TrItem parentItem;

        /// <summary>
        /// Holds the container's internal list of items.
        /// </summary>
        private protected List<TrItem> itemList;

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TrContainer"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parentItem">The container's parent item: No container can be instantiated without a known parent item.</param>
        public TrContainer(TrItem parentItem)
        {
            itemList = new List<TrItem>();
            ParentItem = parentItem;
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
        /// Implementation regarding IEnumerable.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)itemList).GetEnumerator();
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties
       
        /// <summary>
        /// Gets or sets the container's parent item (of type TrItem or derived).
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws exception if set to null.</exception>
        public TrItem ParentItem 
        { 
            get 
            {
                return parentItem;
            } 
        
            set
            {
                parentItem = value;
                if (parentItem == null)
                {
                    throw new ArgumentNullException("A container's parent item can't be null.");
                }

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the container has changed since it was loaded (saved).
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

                ParentItem.HasChanged = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether any changes to the container has has been uploaded.
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

                ParentItem.IsChangesUploaded = value;
            }
        }

        /// <summary>
        /// Gets the number of items in the container.
        /// </summary>
        public int Count { get => itemList.Count; }

        // ------------------------------------------------------------------------------------------------------------------------
        // 10. Indexers 

        /// <summary>
        /// Returns a TrItem element via the indexer.
        /// </summary>
        /// <param name="index">The index of the item in question.</param>
        public TrItem this[int index]
        {
            get { return itemList[index]; }
            set { itemList[index] = value; }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 11. Methods 
        
        /// <summary>
        /// Determines whether an element TrItem is in the container.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <returns>true, if the element is found in the list; otherwise false.</returns>
        public bool Contains(TrItem item)
        {
            return itemList.Contains(item);
        }

        /// <summary>
        /// Adds an item to the end of the list.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(TrItem item)
        {
            itemList.Add(item);
            item.ParentContainer = this;
            ParentItem.HasChanged = true;
        }

        /// <summary>
        /// Removes the first occurrence of the item from the list.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void Remove(TrItem item)
        {
            itemList.Remove(item);
            ParentItem.HasChanged = true;
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="i">The specified index.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws an exception if the index is out of range.</exception>
        public void RemoveAt(int i)
        {
            itemList.RemoveAt(i);
            ParentItem.HasChanged = true;
        }

        /// <summary>
        /// Sorts the items in the list, using the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws an exception if the operation can't be done.</exception>
        public void Sort()
        {
            itemList.Sort();
            ParentItem.HasChanged = true;
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            itemList.Clear();
            ParentItem.HasChanged = true;
        }

        /// <summary>
        /// Searches the container for an item with a certain ID number.
        /// </summary>
        /// <param name="searchID">The ID number to search for.</param>
        /// <returns>An item that matches this ID.</returns>
        /// <exception cref="ArgumentException">Throws exception when an item with this ID isn't in the container.</exception>
        private protected TrItem GetItemFromID(string searchID)
        {
            try
            {
                var item = itemList.Where(c => c.IDNumber == searchID).FirstOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw new ArgumentException($"Couldn't find an item with ID = {searchID}");
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 
    }
}
