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
    /// Interaction logic for dlgShowTextLine.xaml
    /// </summary>
    public partial class dlgShowTextLine : Window
    {
        private TrTextLine CurrentLine;

        private TrPage ParentPage;
        private BitmapImage FullPage;
        private Int32Rect CropRect;
        private CroppedBitmap LineImage;

        public dlgShowTextLine(TrTextLine Line) // , TrPage Page, HttpClient Client
        {
            InitializeComponent();
            CurrentLine = Line;
            DataContext = this.CurrentLine;

            ParentPage = Line.ParentRegion.ParentTranscript.ParentPage;

            FullPage = new BitmapImage();
            FullPage.BeginInit();
            FullPage.CacheOption = BitmapCacheOption.OnLoad;
            FullPage.UriSource = new Uri(ParentPage.ImageURL);
            FullPage.EndInit();

            FullPage.DownloadCompleted += new EventHandler(
                (object xsender, EventArgs xe) =>
                {
                    BitmapImage readySrc = (BitmapImage)xsender;

                    CropRect = new Int32Rect(Line.LeftBorder, Line.TopBorder, Line.BoundingBoxWidth, Line.BoundingBoxHeight);
                    LineImage = new CroppedBitmap(readySrc, CropRect);

                    imgTextLine.Source = LineImage;
                    imgTextLine.Stretch = Stretch.UniformToFill;
                });



        }
    }
}
