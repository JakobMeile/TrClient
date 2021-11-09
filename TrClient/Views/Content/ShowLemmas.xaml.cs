// <copyright file="ShowLemmas.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using TranskribusClient.Core;
    using TranskribusClient.Extensions;

    /// <summary>
    /// Interaction logic for ShowLemmas.xaml.
    /// </summary>
    public partial class ShowLemmas : Window
    {
        GridViewColumnHeader lastHeaderClicked = null;
        ListSortDirection lastDirection = ListSortDirection.Ascending;

        private TrDocument currentDocument;

        //private TrLemmas Lemmas = new TrLemmas();
        public ShowLemmas(TrDocument document)
        {
            InitializeComponent();
            currentDocument = document;

            //Lemmas = Document.Lemmas;
            lvLemmas.ItemsSource = currentDocument.Lemmas;
        }

        private void LvLemmasColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (lastDirection == ListSortDirection.Ascending)
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
                    if (lastHeaderClicked != null && lastHeaderClicked != headerClicked)
                    {
                        lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    lastHeaderClicked = headerClicked;
                    lastDirection = direction;
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
            DialogResult = true;
        }

        private void LvLemmas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(ListViewItem))
            {
                TrLemma lemma = ((ListViewItem)sender).Content as TrLemma;
                if (lemma != null)
                {
                    EditLemma DlgEdit = new EditLemma(lemma);
                    DlgEdit.Owner = this;
                    DlgEdit.ShowDialog();
                    if (DlgEdit.DialogResult == true)
                    {
                        lemma = DlgEdit.CurrentLemma;
                    }
                }
            }
        }

        //private void BtnExport_Click(object sender, RoutedEventArgs e)
        //{   // TrLibrary.ExportFolder +
        //    string FileName = CurrentDocument.ParentCollection.Name + "_" + CurrentDocument.Title + "_"
        //                    + "Lemmas_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".xlsx";

        //    clsTrExcelSheet Sheet = new clsTrExcelSheet();

        //    List<string> Headers = new List<string> { "Content", "Stripped", "Count" };
        //    // List<string> Headers = CurrentPage.GetParagraphs().GetNames(); // CurrentDocument.GetStructuralTags();
        //    Sheet.AddHeaders(Headers);

        //    foreach(TrLemma trLemma in CurrentDocument.Lemmas)
        //        Sheet.AddLemma(trLemma);

        //}
    }
}
