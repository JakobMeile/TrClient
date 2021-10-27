// <copyright file="TrRegion2.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrRegion2.
/// </summary>

namespace TrClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for TrTextRegion and TrTableRegion.
    /// Inherits <see cref="TrPageLevelItem"/>.
    /// </summary>
    public abstract class TrRegion2 : TrPageLevelItem
    {
        public TrRegion2(TrContainer parentContainer) 
            : base(parentContainer)
        {

        }

    }
}
