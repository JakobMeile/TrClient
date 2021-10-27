// <copyright file="ChooseMinMaxNumbers.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for ChooseMinMaxNumbers.xaml.
    /// </summary>
    public partial class ChooseMinMaxNumbers : Window
    {
        public int Minimum = 0;
        public int Maximum = 0;

        public ChooseMinMaxNumbers()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtMinimum.Text != null)
            {
                if (txtMinimum.Text != string.Empty)
                {
                    Minimum = Convert.ToInt32(txtMinimum.Text);
                }
            }

            if (txtMaximum.Text != null)
            {
                if (txtMaximum.Text != string.Empty)
                {
                    Maximum = Convert.ToInt32(txtMaximum.Text);
                }
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
