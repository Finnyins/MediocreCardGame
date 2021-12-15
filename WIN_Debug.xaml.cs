// Finn O'Brien
// Project 24 (Debug Window)
// 12/15/2021


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

namespace Project24
{
    /// <summary>
    /// Interaction logic for WIN_Debug.xaml
    /// </summary>
    public partial class WIN_Debug : Window
    {
        public WIN_Debug()
        {
            InitializeComponent();
            this.Visibility = Visibility.Visible;
        }

        public void DebugWriteLine(string text)
        {
            TBX_Debug.Text += text + "\n";
        }
    }
}
