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
using cafe_management.DTO;
using cafe_management.Helper;
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
            LoadRoleToCmb();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text; //Admin or Staff
            string pass = "1234";//txtPassWord.Password;
            Account account = AccountController.Instance.Login(user, pass);
            if (account != null)
            {
                // Đăng nhập thành công
                // Chuyển đến giao diện chính của ứng dụng
                UserSession.CurrentUser = account;
                MainWindow main = (MainWindow)Window.GetWindow(this);
                main.LoadLayout(new HomeLayout());
            }
            else
            {
                // Đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadRoleToCmb()
        {
            cbUser.ItemsSource = AccountController.Instance.GetUserRole();
            if (cbUser.Items.Count > 0)
            {
                cbUser.SelectedIndex = 0;
            }
        }
        private void buttonForgotpassword_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
