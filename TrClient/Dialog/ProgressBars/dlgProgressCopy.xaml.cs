﻿using System;
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
using System.ComponentModel;
using System.Threading;

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
    /// Interaction logic for dlgProgressCopy.xaml
    /// </summary>
    public partial class dlgProgressCopy : Window
    {
        public dlgProgressCopy(int Maximum)
        {
            InitializeComponent();
            barLoadedDocuments.Minimum = 0;
            barLoadedDocuments.Maximum = Maximum;
        }
    }
}