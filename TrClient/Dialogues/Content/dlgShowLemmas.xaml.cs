using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using TrClient;

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgShowLemmas.xaml
    /// </summary>
    public partial class dlgShowLemmas : Window
    {

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private clsTrDocument CurrentDocument;
        //private clsTrLemmas Lemmas = new clsTrLemmas();

        public dlgShowLemmas(clsTrDocument Document)
        {
            InitializeComponent();
            CurrentDocument = Document;
            //Lemmas = Document.Lemmas;
            lvLemmas.ItemsSource = CurrentDocument.Lemmas;

        }

        

        private void lvLemmasColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate = Application.Current.FindResource("ArrowUp") as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate = Application.Current.FindResource("ArrowDown") as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }

        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lvLemmas.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        private void LvLemmas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(ListViewItem))
            {
                clsTrLemma Lemma = ((ListViewItem)sender).Content as clsTrLemma;
                if (Lemma != null)
                {
                    DialogEditLemma dlgEdit = new DialogEditLemma(Lemma);
                    dlgEdit.Owner = this;
                    dlgEdit.ShowDialog();
                    if (dlgEdit.DialogResult == true)
                        Lemma = dlgEdit.CurrentLemma;
                }
            }
        }


        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            string FileName = clsTrLibrary.ExportFolder + CurrentDocument.ParentCollection.Name + "_" + CurrentDocument.Title + "_"
                            + "Lemmas_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".xlsx";

            clsTrExcelSheet Sheet = new clsTrExcelSheet();

            List<string> Headers = new List<string> { "Content", "Stripped", "Count" };
            // List<string> Headers = CurrentPage.GetParagraphs().GetNames(); // CurrentDocument.GetStructuralTags();
            Sheet.AddHeaders(Headers);

            foreach(clsTrLemma trLemma in CurrentDocument.Lemmas)
                Sheet.AddLemma(trLemma);


        }

    }



}
