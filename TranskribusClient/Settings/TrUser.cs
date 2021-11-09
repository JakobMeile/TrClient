// <copyright file="TrUser.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrUser.
/// </summary>

namespace TranskribusClient.Settings
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using TranskribusClient.Core;

    /// <summary>
    /// Class to hold the user's settings, including credentials.
    /// </summary>
    public class TrUser
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        public TrUserSettings UserSettings { get; set; }

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrUser"/> class.
        /// Default constructor.
        /// </summary>
        public TrUser()
        {
            if (UserSettings == null)
            {
                if (File.Exists("UserSettings.xml"))
                {
                    using (var stream = File.OpenRead("UserSettings.xml"))
                    {
                        var serializer = new XmlSerializer(typeof(TrUserSettings));
                        UserSettings = serializer.Deserialize(stream) as TrUserSettings;
                    }
                }
                else
                {
                    UserSettings = new TrUserSettings();
                }
            }


            using (var stream = File.Open("UserSettings.xml", FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(TrUserSettings));
                serializer.Serialize(stream, UserSettings);
            }

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

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 

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
