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

namespace cafe_management.UI
{
    /// <summary>
    /// Interaction logic for HomeLayout.xaml
    /// </summary>
    public partial class HomeLayout : UserControl
    {
        public HomeLayout()
        {
            InitializeComponent();
        }
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)FindResource("AccountTemplate");
        }

        private void TableButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)FindResource("TableTemplate");
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)FindResource("MenuTemplate");
        }

        private void BillButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)FindResource("BillTemplate");
        }

        private void ButtonExitAcc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddTable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteTable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonEditTable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddFood_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteFood_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
