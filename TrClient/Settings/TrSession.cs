// <copyright file="TrSession.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public class TrSession.
/// </summary>

/// <summary>
/// Class to hold the current session.
/// </summary>
namespace TranskribusClient.Settings
{
    using System;
    using System.Net.Http;
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
    /// Class to keep the current session, combining the user, the HttpClient and the user's collections.
    /// </summary>
    public class TrSession
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 
        public TrUser currentUser { get; set; }
        
        public static XmlDocument collectionsDocument = new XmlDocument();
        
        public ObservableCollection<TrCollection> userCollections { get; set; }
                
        public static readonly HttpClient httpClient = new HttpClient();
         
        private string trpServerBaseAddress = Properties.Resources.TrpServerBaseAddress;

        private static string trpLogin = Properties.Resources.TrpServerBaseAddress + Properties.Resources.TrpServerPathLogin;

        private static string trpCollections = Properties.Resources.TrpServerBaseAddress + Properties.Resources.TrpServerPathCollectionsList;

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrSession"/> class.
        /// </summary>
        /// <param name="user">The user to log in.</param>
        public TrSession(TrUser user)
        {
            if (user != null)
            {
                try
                {
                    httpClient.BaseAddress = new Uri(trpServerBaseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();

                    RunLoginAndGetMyCollections(currentUser);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrSession"/> class.
        /// Default constructor.
        /// </summary>
        public TrSession()
        {

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

        public async void RunLoginAndGetMyCollections(TrUser user)
        {
            // Kaldes KUN i online-mode!!
            // Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            var encodedCredentials = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user", user.UserSettings.Username),
                    new KeyValuePair<string, string>("pw", user.UserSettings.Password),
                });

            try
            {
                HttpResponseMessage loginResponseMessage = await httpClient.PostAsync(trpLogin, encodedCredentials);

                string loginResponse = loginResponseMessage.StatusCode.ToString();

                bool status = loginResponse == "OK";
                if (status)
                {
                    // Henter brugerens collections ind i et XMLdoc
                    HttpResponseMessage collectionsResponseMessage = await httpClient.GetAsync(trpCollections);
                    string collectionsResponse = await collectionsResponseMessage.Content.ReadAsStringAsync();
                    collectionsDocument.LoadXml(collectionsResponse);

                    // Udtrækker de enkelte collections
                    XmlNodeList collectionNodes = collectionsDocument.DocumentElement.SelectNodes("//trpCollection");
                    foreach (XmlNode xnCollection in collectionNodes)
                    {
                        XmlNodeList collectionMetaData = xnCollection.ChildNodes;
                        string colID = string.Empty;
                        string colName = string.Empty;
                        int nrOfDocs = 0;

                        foreach (XmlNode xnCollectionMetaData in collectionMetaData)
                        {
                            string name = xnCollectionMetaData.Name;
                            string value = xnCollectionMetaData.InnerText;

                            switch (name)
                            {
                                case "colId":
                                    colID = value;
                                    break;
                                case "colName":
                                    colName = value;
                                    break;
                                case "nrOfDocuments":
                                    nrOfDocs = Int32.Parse(value);
                                    break;
                            }
                        }

                        TrCollection coll = new TrCollection(colName, colID, nrOfDocs);
                        userCollections.Add(coll);
                    }
                }

                // userCollections.Sort();

                // fylder box op
                // lstCollections.ItemsSource = MyCollections;
                // MyCollections.IsLoaded = true;
                // Mouse.OverrideCursor = null;
            }
            catch (TaskCanceledException e)
            {
                // MessageBox.Show("Exception occured!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Debug.WriteLine($"Task canceled! Exception message when logging in: {e.Message}");
            }
            catch (OperationCanceledException e)
            {
                Debug.WriteLine($"Operation canceled! Exception message when logging in: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"General error! Exception message when logging in: {e.Message}");
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 

    }
}
