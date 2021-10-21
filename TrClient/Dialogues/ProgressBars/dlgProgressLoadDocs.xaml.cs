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
using System.ComponentModel;
using System.Threading;

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgProgressLoadDocs.xaml
    /// </summary>
    public partial class dlgProgressLoadDocs : Window
    {
        public dlgProgressLoadDocs(int Maximum)
        {
            InitializeComponent();

            barLoadedDocuments.Minimum = 0;
            barLoadedDocuments.Maximum = Maximum;
            
        }
    }
}
