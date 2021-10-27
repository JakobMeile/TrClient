// <copyright file="TrUser.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Settings
{
    using System;

    [Serializable]
    public class TrUser
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
