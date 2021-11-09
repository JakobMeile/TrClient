// <copyright file="EditTextLine.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using TranskribusClient.Core;

    /// <summary>
    /// Interaction logic for EditTextLine.xaml.
    /// </summary>
    public partial class EditTextLine : Window
    {
        public TrTextLine CurrentLine;
        private string oldText;
        private string oldCoordsString;

        private TrPage parentPage;

        private CroppedBitmap croppedImage;
        private TransformedBitmap scaledImage;

        private int newHeight = 200;
        private double scaleFactor;

        public EditTextLine(TrTextLine textLine)
        {
            InitializeComponent();
            CurrentLine = textLine;
            oldText = CurrentLine.TextEquiv;
            oldCoordsString = CurrentLine.BaseLineCoordsString;

            DataContext = CurrentLine;

            lblExpandedText.Content = CurrentLine.ExpandedText;

            parentPage = textLine.ParentRegion.ParentTranscript.ParentPage;

            if (CurrentLine.HasSpecificStructuralTag("CriticalBaseLineError"))
            {
                btnMarkCritical.Content = "UN-Mark as Critical";
            }

            Debug.Print($"New TextLine: ParentPage = {parentPage.PageNr}, LineNr = {textLine.Number}, TextEquiv = {textLine.TextEquiv}");
            Debug.Print($"BaselineCoords: {textLine.BaseLineCoordsString}");
            Int32Rect testBB = textLine.BoundingBoxLarge;
            Debug.Print($"Boundingbox: X: {testBB.X}, Y: {testBB.Y}, W: {testBB.Width}, H: {testBB.Height}");

            if (!parentPage.IsPageImageLoaded)
            {
                parentPage.LoadImage();
                parentPage.PageImage.DownloadCompleted += new EventHandler(
                    (object xsender, EventArgs xe) =>
                    {
                        BitmapImage readySrc = (BitmapImage)xsender;
                        croppedImage = new CroppedBitmap(readySrc, textLine.BoundingBoxLarge);

                        //ScaleFactor = NewHeight / CroppedImage.PixelHeight;
                        //ScaledImage = new TransformedBitmap(CroppedImage, new ScaleTransform(ScaleFactor, ScaleFactor));
                        imgTextLine.Source = croppedImage;

                        //imgTextLine.Source = ScaledImage;
                    });
            }
            else
            {
                croppedImage = new CroppedBitmap(parentPage.PageImage, textLine.BoundingBoxLarge);

                //ScaleFactor = NewHeight / CroppedImage.PixelHeight;
                //ScaledImage = new TransformedBitmap(CroppedImage, new ScaleTransform(ScaleFactor, ScaleFactor));
                imgTextLine.Source = croppedImage;

                //imgTextLine.Source = ScaledImage;
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
