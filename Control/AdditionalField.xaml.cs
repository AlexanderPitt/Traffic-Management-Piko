using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PikoTrafficManager.Control
{
    /// <summary>
    /// Logique d'interaction pour AdditionalField.xaml
    /// </summary>
    public partial class AdditionalField : UserControl
    {
        public AdditionalField()
        {
            InitializeComponent();
        }

        private void btChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                tbValue.Text = openFileDialog.FileName;
        }


    }
}
