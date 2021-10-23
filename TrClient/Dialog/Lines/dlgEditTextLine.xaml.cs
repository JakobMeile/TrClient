using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Diagnostics;
using TrClient.Core;
using TrClient.Dialog;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Dialog
{
    /// <summary>
    /// Interaction logic for dlgEditTextLine.xaml
    /// </summary>
    public partial class dlgEditTextLine : Window
    {
        public TrTextLine CurrentLine;
        private string OldText;
        private string OldCoordsString;

        private TrPage ParentPage;

        private CroppedBitmap CroppedImage;
        private TransformedBitmap ScaledImage;

        private int NewHeight = 200;
        private double ScaleFactor;

        public dlgEditTextLine(TrTextLine TL)
        {
            InitializeComponent();
            CurrentLine = TL;
            OldText = CurrentLine.TextEquiv;
            OldCoordsString = CurrentLine.BaseLineCoordsString;

            DataContext = CurrentLine;

            lblExpandedText.Content = CurrentLine.ExpandedText;

            ParentPage = TL.ParentRegion.ParentTranscript.ParentPage;

            if (CurrentLine.HasSpecificStructuralTag("CriticalBaseLineError"))
                btnMarkCritical.Content = "UN-Mark as Critical";

            Debug.Print($"New TextLine: ParentPage = {ParentPage.PageNr}, LineNr = {TL.Number}, TextEquiv = {TL.TextEquiv}");
            Debug.Print($"BaselineCoords: {TL.BaseLineCoordsString}");
            Int32Rect TestBB = TL.BoundingBoxLarge;
            Debug.Print($"Boundingbox: X: {TestBB.X}, Y: {TestBB.Y}, W: {TestBB.Width}, H: {TestBB.Height}");

            if (!ParentPage.IsPageImageLoaded)
            {
                ParentPage.LoadImage();
                ParentPage.PageImage.DownloadCompleted += new EventHandler(
                    (object xsender, EventArgs xe) =>
                    {
                        BitmapImage readySrc = (BitmapImage)xsender;
                        CroppedImage = new CroppedBitmap(readySrc, TL.BoundingBoxLarge);
                        //ScaleFactor = NewHeight / CroppedImage.PixelHeight;
                        //ScaledImage = new TransformedBitmap(CroppedImage, new ScaleTransform(ScaleFactor, ScaleFactor));
                        imgTextLine.Source = CroppedImage;
                        //imgTextLine.Source = ScaledImage;
                    });
            }
            else
            {
                CroppedImage = new CroppedBitmap(ParentPage.PageImage, TL.BoundingBoxLarge);
                //ScaleFactor = NewHeight / CroppedImage.PixelHeight;
                //ScaledImage = new TransformedBitmap(CroppedImage, new ScaleTransform(ScaleFactor, ScaleFactor));
                imgTextLine.Source = CroppedImage;
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

            DrawingImage CombinedImage = new DrawingImage();

            ImageDrawing Img = new ImageDrawing();
            Img.ImageSource = CroppedImage;

            DrawingGroup ImageGroup = new DrawingGroup();
            ImageGroup.Children.Add(Img);
            


        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != OldText)
            {
                CurrentLine.TextEquiv = txtRawText.Text;
                CurrentLine.HasChanged = true;
            }

            if (CurrentLine.BaseLineCoordsString != OldCoordsString)
            {
                CurrentLine.HasChanged = true;
            }


            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != OldText)
            {
                CurrentLine.TextEquiv = OldText;
                CurrentLine.HasChanged = false;
            }

            if (CurrentLine.BaseLineCoordsString != OldCoordsString)
            {
                CurrentLine.BaseLineCoordsString = OldCoordsString;
                CurrentLine.HasChanged = false;
            }

            this.DialogResult = false;
        }

        private void TxtRawText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtRawText.Text != OldText)
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

                Polyline NewBL = CurrentLine.VisualBaseLine;
                NewBL.Stroke = Brushes.OrangeRed;
                NewBL.StrokeThickness = 2;
                imgCanvas.Children.Add(NewBL);
           }
        }

        private void BtnFlatten_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentLine.IsBaseLineStraight)
            {
                CurrentLine.FlattenBaseLine();
                txtBaseLineCoordsString.Text = CurrentLine.BaseLineCoordsString;
                pliBaseLine.StrokeThickness = 0;

                Polyline NewBL = CurrentLine.VisualBaseLine;
                NewBL.Stroke = Brushes.OrangeRed;
                NewBL.StrokeThickness = 2;
                imgCanvas.Children.Add(NewBL);

            }
        }

        private void BtnFixDir_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentLine.IsBaseLineDirectionOK)
            {
                CurrentLine.FixDirection();
                txtBaseLineCoordsString.Text = CurrentLine.BaseLineCoordsString;
                pliBaseLine.StrokeThickness = 0;

                Polyline NewBL = CurrentLine.VisualBaseLine;
                NewBL.Stroke = Brushes.OrangeRed;
                NewBL.StrokeThickness = 2;
                imgCanvas.Children.Add(NewBL);

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
        //    dlgShowTextLine dlgShowTextLine = new dlgShowTextLine(CurrentLine);
        //    dlgShowTextLine.Owner = this;
        //    dlgShowTextLine.ShowDialog();
        //}
    }
}
