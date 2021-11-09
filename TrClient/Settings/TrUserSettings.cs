// <copyright file="TrUserSettings.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Settings
{
    using System;

    [Serializable]
    public class TrUserSettings
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool RememberCredentials { get; set; }

        public bool AutoLogin { get; set; }
    }
}
