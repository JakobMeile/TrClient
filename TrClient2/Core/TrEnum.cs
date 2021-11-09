// <copyright file="TrEnum.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains enumeration types for project TrClient.
/// </summary>

namespace TrClient2.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Enumerations used in project TrClient.
    /// </summary>
    public static class TrEnum
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 7. Enums 

        public enum Status
        {
            Unloaded,
            Loaded,
            Changed,
            Uploaded,
            Refreshed
        }



    }
}
