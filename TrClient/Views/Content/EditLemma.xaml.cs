// <copyright file="EditLemma.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;
    using TrClient.Extensions;

    /// <summary>
    /// Interaction logic for EditLemma.xaml.
    /// </summary>
    public partial class EditLemma : Window
    {
        public TrLemma CurrentLemma;
        private string oldText;

        public EditLemma(TrLemma lemma)
        {
            InitializeComponent();

            CurrentLemma = lemma;
            oldText = CurrentLemma.Content;

            DataContext = CurrentLemma;

            Loaded += Window_Loaded;

            //lblExpandedText.Content = CurrentLemma.GetExpandedText(true, false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtRawText.Focusable = true;
            txtRawText.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != oldText)
            {
                CurrentLemma.Content = txtRawText.Text;
                CurrentLemma.HasChanged = true;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != oldText)
            {
                CurrentLemma.Content = oldText;
                CurrentLemma.HasChanged = false;
            }

            DialogResult = false;
        }

        //private void TxtRawText_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (txtRawText.Text != OldText)
        //    {
        //        CurrentLemma.Content = txtRawText.Text;
        //        CurrentLemma.HasChanged = true;
        //    }
        //}
    }
}
