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
using System.Text.RegularExpressions;
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
    /// Interaction logic for dlgFilterLinesByRegex.xaml
    /// </summary>
    public partial class dlgFilterLinesByRegex : Window
    {
        private TrDocument CurrentDocument;
        private TrTextLines Lines = new TrTextLines();
        private HttpClient CurrentClient;

        private string RegexPattern = "";

        private string TagName;
        private bool OverWrite = false;

        public dlgFilterLinesByRegex(TrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;
            txtPattern.Text = "";
            //DataContext = this.FilterWindow;
            //GetLines();

        }

        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterWindow.LeftBorder}, Right: {FilterWindow.RightBorder}, " +
            //    $"Top: {FilterWindow.TopBorder}, Bottom: {FilterWindow.BottomBorder}, Inside: {FilterWindow.Inside}");

            if (txtPattern.Text != "")
            {
                if (TrLibrary.VerifyRegex(RegexPattern))
                {
                    Lines.Clear();
                    lstLines.ItemsSource = null;

                    // Regex MatchPattern = new Regex(RegexPattern);

                    TrRegion_Text TR;
                    TrTextLines TempLines = CurrentDocument.GetLines_RegexFiltered(RegexPattern);

                    foreach (TrTextLine TL in TempLines)
                    {
                        TR = TL.ParentRegion;
                        Lines.Add(TL);
                        TL.ParentRegion = TR;
                    }
                    lstLines.ItemsSource = Lines;
                    Debug.WriteLine($"Line count: {Lines.Count}");

                }
                else
                    Debug.Print("Wrong regex!");

            }

        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            RegexPattern = string.Format(@"({0})", txtPattern.Text.Trim());
            //string escape = Regex.Escape(RegexPattern);
            //string unEsc = ""; // Regex.Unescape(RegexPattern);    
            Debug.Print($"Regex Pattern = _{RegexPattern}_"); // , esc: _{escape}_, unesc: _{unEsc}_
            GetLines();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            //FilterWindow.Reset();
            RegexPattern = "";
            Lines.Clear();
            lstLines.ItemsSource = null;
            //GetLines();
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
