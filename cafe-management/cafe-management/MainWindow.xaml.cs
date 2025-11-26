using cafe_management.DAO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cafe_management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var un = unBox.Text;
            var pw = pwBox.Text;
            // Tìm tài khoản trong cơ sở dữ liệu
            if (AccountController.Instance.login(un, pw))
            {
                MessageBox.Show("To ADMIN GUI");
                return;
            }
            MessageBox.Show("Sai tài khoản hoặc mật khẩu!!!");
        }
    }
}