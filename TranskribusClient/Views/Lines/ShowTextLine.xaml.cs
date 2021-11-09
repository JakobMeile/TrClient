// <copyright file="ShowTextLine.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using TranskribusClient.Core;

    /// <summary>
    /// Interaction logic for ShowTextLine.xaml.
    /// </summary>
    public partial class ShowTextLine : Window
    {
        private TrTextLine currentLine;

        private TrPage parentPage;
        private BitmapImage fullPage;
        private Int32Rect cropRect;
        private CroppedBitmap lineImage;

        public ShowTextLine(TrTextLine line) // , TrPage Page, HttpClient httpClient
        {
            InitializeComponent();
            currentLine = line;
            DataContext = currentLine;

            parentPage = line.ParentRegion.ParentTranscript.ParentPage;

            fullPage = new BitmapImage();
            fullPage.BeginInit();
            fullPage.CacheOption = BitmapCacheOption.OnLoad;
            fullPage.UriSource = new Uri(parentPage.ImageURL);
            fullPage.EndInit();

            fullPage.DownloadCompleted += new EventHandler(
                (object xsender, EventArgs xe) =>
                {
                    BitmapImage readySrc = (BitmapImage)xsender;

                    cropRect = new Int32Rect(line.LeftBorder, line.TopBorder, line.BoundingBoxWidth, line.BoundingBoxHeight);
                    lineImage = new CroppedBitmap(readySrc, cropRect);

                    imgTextLine.Source = lineImage;
                    imgTextLine.Stretch = Stretch.UniformToFill;
                });
        }
    }
}
