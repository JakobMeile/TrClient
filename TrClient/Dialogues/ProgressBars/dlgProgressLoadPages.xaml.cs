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

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgProgressLoadPages.xaml
    /// </summary>
    public partial class dlgProgressLoadPages : Window
    {
        public dlgProgressLoadPages(int Maximum)
        {
            InitializeComponent();

            barLoadedPages.Minimum = 0;
            barLoadedPages.Maximum = Maximum;

        }
    }
}
