using cafe_management.DAO;
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
    /// Interaction logic for LoginLayout.xaml
    /// </summary>
    public partial class LoginLayout : UserControl
    {
        public LoginLayout()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            string pass = txtPassWord.Password;
            if (AccountController.Instance.login(user, pass))
            {
                // Đăng nhập thành công
                // Chuyển đến giao diện chính của ứng dụng
                MainWindow main = (MainWindow)Window.GetWindow(this);
                main.LoadHomeLayout();
            }
            else
            {
                // Đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
                                       
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
