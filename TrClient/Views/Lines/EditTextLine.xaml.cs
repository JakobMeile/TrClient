// <copyright file="EditTextLine.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Media;
    using System.Xml;
    using System.Net.Http;
    using TrClient.Core;
    using TrClient.Libraries;

    /// <summary>
    /// Interaction logic for EditTextLine.xaml.
    /// </summary>
    public partial class EditTextLine : Window
    {
        public TrTextLine CurrentLine;

        private HttpClient currentClient;

        private string oldText;
        private string oldCoordsString;

        private TrPage parentPage;

        private CroppedBitmap croppedImage;
        private TransformedBitmap scaledImage;

        private int newHeight = 200;
        private double scaleFactor;

        public EditTextLine(TrTextLine textLine, HttpClient client)
        {
            InitializeComponent();
            CurrentLine = textLine;
            currentClient = client;
            oldText = CurrentLine.TextEquiv;
            oldCoordsString = CurrentLine.BaseLineCoordsString;

            DataContext = CurrentLine;

            lblExpandedText.Content = CurrentLine.ExpandedText;

            parentPage = textLine.ParentRegion.ParentTranscript.ParentPage;

            if (CurrentLine.HasSpecificStructuralTag("CriticalBaseLineError"))
            {
                btnMarkCritical.Content = "UN-Mark as Critical";
            }

            Int32Rect testBB = textLine.BoundingBoxLarge;

            Debug.Print($"    ");
            Debug.Print($"EDIT TEXTLINE: ParentPage = {parentPage.PageNr}, LineNr = {textLine.Number}, TextEquiv = {textLine.TextEquiv}");
            Debug.Print($"BaselineCoords: {textLine.BaseLineCoordsString}");
            Debug.Print($"Boundingbox: X: {testBB.X}, Y: {testBB.Y}, W: {testBB.Width}, H: {testBB.Height}");

            string error = String.Empty;

            if (!parentPage.IsPageImageLoaded)
            {
                Debug.Print($"ParentPage: Image is NOT loaded.");

                // await LoadImage();

                // Task<bool> imageLoaded = parentPage.LoadImage(currentClient);
                
                //bool OK = await imageLoaded;

                parentPage.LoadImage(currentClient);

                try
                {
                    parentPage.PageImage.DownloadCompleted += new EventHandler(
                        (object xsender, EventArgs xe) =>
                        {
                            BitmapImage readySrc = (BitmapImage)xsender;

                            try
                            {
                                croppedImage = new CroppedBitmap(readySrc, textLine.BoundingBoxLarge);

                                //ScaleFactor = NewHeight / CroppedImage.PixelHeight;
                                //ScaledImage = new TransformedBitmap(CroppedImage, new ScaleTransform(ScaleFactor, ScaleFactor));
                                imgTextLine.Source = croppedImage;

                                //imgTextLine.Source = ScaledImage;
                            }
                            catch (Exception e)
                            {
                                error = $"EditTextLine: Error cropping un-loaded image on page {parentPage.PageNr}; message: {e.Message}";
                                Debug.WriteLine(error);
                                MessageBox.Show(error, TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                //throw e;
                            }
                        });
                }
                catch (Exception e) 
                {
                    error = $"EditTextLine: Error loading AND cropping image on page {parentPage.PageNr}; message: {e.Message}";
                    Debug.WriteLine(error);
                    MessageBox.Show(error, TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    //throw e;
                }

            }
            else
            {
                Debug.Print($"ParentPage: Image IS loaded: URI = {parentPage.PageImage}");
                
                if (parentPage.PageImage != null)
                {
                    try
                    {
                        while (parentPage.PageImage.IsDownloading)
                        {
                            Debug.Print("But ... It is still downloading...");   
                        }

                        croppedImage = new CroppedBitmap(parentPage.PageImage, textLine.BoundingBoxLarge);

                        //ScaleFactor = NewHeight / CroppedImage.PixelHeight;
                        //ScaledImage = new TransformedBitmap(CroppedImage, new ScaleTransform(ScaleFactor, ScaleFactor));
                        imgTextLine.Source = croppedImage;

                        //imgTextLine.Source = ScaledImage;
                    }
                    catch (Exception e)
                    {
                        error = $"EditTextLine: Error cropping already loaded image on page {parentPage.PageNr}; message: {e.Message}";
                        Debug.WriteLine(error);
                        MessageBox.Show(error, TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        //throw e;
                    }
                }
                else
                {
                    Debug.Print($"ParentPage: Image was NULL...");
                }
            }

            // så tegner vi LineArea og Baseline
            plyLineArea = CurrentLine.VisualLineArea;
            plyLineArea.Stroke = Brushes.AliceBlue;
            plyLineArea.StrokeThickness = 1;
            imgCanvas.Children.Add(plyLineArea);

            pliBaseLine = CurrentLine.VisualBaseLine;
            pliBaseLine.Stroke = Brushes.OrangeRed;
            pliBaseLine.StrokeThickness = 2;
            imgCanvas.Children.Add(pliBaseLine);

            DrawingImage combinedImage = new DrawingImage();

            ImageDrawing img = new ImageDrawing();
            img.ImageSource = croppedImage;

            DrawingGroup imageGroup = new DrawingGroup();
            imageGroup.Children.Add(img);
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != oldText)
            {
                CurrentLine.TextEquiv = txtRawText.Text;
                CurrentLine.HasChanged = true;
            }

            if (CurrentLine.BaseLineCoordsString != oldCoordsString)
            {
                CurrentLine.HasChanged = true;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != oldText)
            {
                CurrentLine.TextEquiv = oldText;
                CurrentLine.HasChanged = false;
            }

            if (CurrentLine.BaseLineCoordsString != oldCoordsString)
            {
                CurrentLine.BaseLineCoordsString = oldCoordsString;
                CurrentLine.HasChanged = false;
            }

            DialogResult = false;
        }

        private void TxtRawText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtRawText.Text != oldText)
            {
                CurrentLine.TextEquiv = txtRawText.Text;
                lblExpandedText.Content = CurrentLine.ExpandedText;
                CurrentLine.HasChanged = true;
            }
        }

        private void BtnLimitCoords_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentLine.IsCoordinatesPositive)
            {
                CurrentLine.LimitCoordsToPage();
                txtBaseLineCoordsString.Text = CurrentLine.BaseLineCoordsString;
                pliBaseLine.StrokeThickness = 0;

                Polyline newBL = CurrentLine.VisualBaseLine;
                newBL.Stroke = Brushes.OrangeRed;
                newBL.StrokeThickness = 2;
                imgCanvas.Children.Add(newBL);
            }
        }

        private void BtnFlatten_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentLine.IsBaseLineStraight)
            {
                CurrentLine.FlattenBaseLine();
                txtBaseLineCoordsString.Text = CurrentLine.BaseLineCoordsString;
                pliBaseLine.StrokeThickness = 0;

                Polyline newBL = CurrentLine.VisualBaseLine;
                newBL.Stroke = Brushes.OrangeRed;
                newBL.StrokeThickness = 2;
                imgCanvas.Children.Add(newBL);
            }
        }

        private void BtnFixDir_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentLine.IsBaseLineDirectionOK)
            {
                CurrentLine.FixDirection();
                txtBaseLineCoordsString.Text = CurrentLine.BaseLineCoordsString;
                pliBaseLine.StrokeThickness = 0;

                Polyline newBL = CurrentLine.VisualBaseLine;
                newBL.Stroke = Brushes.OrangeRed;
                newBL.StrokeThickness = 2;
                imgCanvas.Children.Add(newBL);
            }
        }

        private void BtnMarkCritical_Click(object sender, RoutedEventArgs e)
        {
            // har den allerede?
            if (CurrentLine.HasSpecificStructuralTag("CriticalBaseLineError"))
            {
                CurrentLine.DeleteStructuralTag();
                btnMarkCritical.Content = "Mark as Critical";
            }
            else
            {
                CurrentLine.AddStructuralTag("CriticalBaseLineError", true);
                btnMarkCritical.Content = "UN-Mark as Critical";
            }
        }

        //private void BtnShow_Click(object sender, RoutedEventArgs e)
        //{
        //    ShowTextLine ShowTextLine = new ShowTextLine(CurrentLine);
        //    ShowTextLine.Owner = this;
        //    ShowTextLine.ShowDialog();
        //}
    }
}
