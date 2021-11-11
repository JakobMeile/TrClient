// <copyright file="TrCollectionsNew.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrCollectionsNew.
/// </summary>

namespace TrClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Container class for collections.
    /// Inherits <see cref="TrContainer"/>.
    /// </summary>
    public class TrCollectionsNew // : TrContainer
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TrCollectionsNew"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="parentItem">The collections parent item: No container can be instantiated without a known parent item.</param>
        public TrCollectionsNew(TrItem parentItem)
            //: base(parentItem)
        {

        }

        ///// <summary>
        ///// Searches the collections for a collection with a certain ID number.
        ///// </summary>
        ///// <param name="searchID">The ID number to search for.</param>
        ///// <returns>A collection that matches this ID.</returns>
        ///// <exception cref="ArgumentException">Throws exception (from base class: GetItemFromID) when an item with this ID isn't in the container.</exception>
        // public TrItem GetCollectionFromID(string searchID)
        // {
        //     return GetItemFromID(searchID);
        // }
    }
}
