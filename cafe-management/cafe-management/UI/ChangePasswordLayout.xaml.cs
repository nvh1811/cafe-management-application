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
    /// Interaction logic for ChangePasswordLayout.xaml
    /// </summary>
    public partial class ChangePasswordLayout : UserControl
    {
        public ChangePasswordLayout()
        {
            InitializeComponent();
        }

        private void buttonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đổi thành công.");
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Window.GetWindow(this);
            main.LoadLayout(new HomeLayout());
        }
    }
}
