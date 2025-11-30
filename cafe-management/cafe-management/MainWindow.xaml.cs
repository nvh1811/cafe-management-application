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
using cafe_management.UI;
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
            // Hiển thị trang đăng nhập
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new LoginLayout());

        }
        public void LoadHomeLayout()
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new HomeLayout());
        }
    }
}