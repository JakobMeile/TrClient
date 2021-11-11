using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrClient2.Core;

namespace TrClient2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            // preparing for test

            TrCollection collection = new TrCollection("My collection", "id123", 12);

            TrDocument document = new TrDocument(collection, "id234", "My document", 13);

            Debug.Print("Before");

            document.Status = TrEnum.Status.Loaded;

            Debug.Print("After");

            //int length = 10;
            //string content;

            //for (int i = 0; i < length; i++)
            //{
            //    TrPage page = new TrPage(document, "id345", i+1, "", "", 1000, 2000);
            //    document.Pages.Add(page);

            //    for (int j = 0; j < 2; j++)
            //    {
            //        TrTranscript transcript = new TrTranscript(page, "id456", "xxx", i+1, "new", "jme", "xxxx");
            //        page.Transcripts.Add(transcript);

            //        for (int k = 0; k < 2; k++)
            //        {
            //            TrTextRegion textRegion = new TrTextRegion(transcript, "id567", "type", 0, 0, "xxxx");
            //            transcript.Regions.Add(textRegion);

            //            for (int l = 0; l < length; l++)
            //            {
            //                content = "Line # " + l.ToString();
            //                TrTextLine normalLine = new TrTextLine(textRegion, "id56", "tags", "coords", "baselindcoords", "content");
            //                textRegion.Lines.Add(normalLine);
            //            }

            //            for (int m = 0; m < 1; m++)
            //            {
            //                TrTableRegion tableRegion = new TrTableRegion(transcript, "id34", "type", 0, 0, "coords");
            //                transcript.Regions.Add(textRegion);

            //                for (int n = 0; n < length; n++)
            //                {
            //                    TrCell cell = new TrCell(tableRegion, "id2", "1", "1", "coords", "corners");
            //                    tableRegion.Cells.Add(cell);

            //                    for (int o = 0; o < length; o++)
            //                    {
            //                        content = "Line (in cell) # " + o.ToString();
            //                        TrTextLine cellLine = new TrTextLine(cell, "id56", "tags", "coords", "baselindcoords", "content");
            //                        cell.TextLines.Add(cellLine);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}


            InitializeComponent();

            //lstLines.ItemsSource = document.Lines;






        }

        private void btnDoSmth_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
