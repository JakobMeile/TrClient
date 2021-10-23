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
    /// Interaction logic for dlgFilterLinesByLocation.xaml
    /// </summary>
    public partial class dlgFilterLinesByLocation : Window
    {
        private TrDocument CurrentDocument;
        private TrTextLines Lines = new TrTextLines();
        private HttpClient CurrentClient;

        // public TrPercentualWindow FilterSettings = new TrPercentualWindow();
        public TrLineFilterSettings FilterSettings = new TrLineFilterSettings();

        private string TagName;
        private bool OverWrite = false;


        public dlgFilterLinesByLocation(TrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;
            DataContext = this.FilterSettings;
            GetLines();
            // FilterSettings = new TrPercentualWindow(0, 0, 0, 0);
        }

        private void RdInside_Checked(object sender, RoutedEventArgs e)
        {
            FilterSettings.Inside = true;
        }

        private void RdOutside_Checked(object sender, RoutedEventArgs e)
        {
            FilterSettings.Inside = false;
        }


        private void SldTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowHeigth = FilterSettings.BottomBorder - FilterSettings.TopBorder;
        }

        private void SldBottom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowHeigth = FilterSettings.BottomBorder - FilterSettings.TopBorder;
        }

        private void SldLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowWidth = FilterSettings.RightBorder - FilterSettings.LeftBorder;
        }

        private void SldRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowWidth = FilterSettings.RightBorder - FilterSettings.LeftBorder;
        }

        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterSettings.LeftBorder}, Right: {FilterSettings.RightBorder}, " +
            //    $"Top: {FilterSettings.TopBorder}, Bottom: {FilterSettings.BottomBorder}, Inside: {FilterSettings.Inside}");
            Lines.Clear();
            lstLines.ItemsSource = null;

            TrRegion_Text TR;
            TrTextLines TempLines = CurrentDocument.GetLines_WindowFiltered(FilterSettings);

            foreach (TrTextLine TL in TempLines)
            {
                TR = TL.ParentRegion;
                Lines.Add(TL);
                TL.ParentRegion = TR;
            }
            lstLines.ItemsSource = Lines;
            Debug.WriteLine($"Line count: {Lines.Count}");

        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            GetLines();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            FilterSettings.Reset();
            GetLines();
        }

        private void ChkOverWrite_Checked(object sender, RoutedEventArgs e)
        {
            OverWrite = true;
        }

        private void ChkOverWrite_Unchecked(object sender, RoutedEventArgs e)
        {
            OverWrite = false;
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Region = GetNumber(cmbRegion.Text);
            //Line = GetNumber(cmbLine.Text);
            TagName = txtTag.Text;

            if (TagName != "")
            {
                foreach (object o in lstLines.SelectedItems)
                    (o as TrTextLine).AddStructuralTag(TagName, OverWrite);
            }
            txtTag.Text = "";
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lstLines.SelectedItems)
                (o as TrTextLine).DeleteStructuralTag();
        }

        private void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            dlgRenameTags dlgRename = new dlgRenameTags(CurrentDocument);
            dlgRename.Owner = this;
            dlgRename.ShowDialog();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            CurrentDocument.Upload(CurrentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lstLines.Items)
                lstLines.SelectedItems.Add(o);
        }

        private void BtnNone_Click(object sender, RoutedEventArgs e)
        {
            lstLines.SelectedItems.Clear();
        }

    }
}
