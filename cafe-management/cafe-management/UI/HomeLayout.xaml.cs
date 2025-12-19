
using cafe_management.DAO;
using cafe_management.DTO;
using cafe_management.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cafe_management.UI
{
    public partial class HomeLayout : UserControl
    {
        public List<Bill> ListBillsReport { get; set; } = new List<Bill>();
        private int _selectedTableId = 0;
        public List<TableInfo> tableList { get; set; } = new List<TableInfo>();

        // Dictionary để lưu trạng thái các bàn
        private Dictionary<int, TableInfo> _tables = new Dictionary<int, TableInfo>();

        // Danh sách menu đồ ăn - DÙNG CHUNG cho cả MenuTemplate và OrderTemplate
        public List<FoodItem> MenuItems { get; set; } = new List<FoodItem>();

        // Order hiện tại
        private List<OrderItem> _currentOrder = new List<OrderItem>();

        // Biến tạm để lưu món đang chỉnh sửa
        private FoodItem? _editingMenuItem = null;

        private int _idBill = 0;

        private int statusBill = 0;

        public HomeLayout()
        {
            InitializeComponent();
            MenuItems = FoodDAO.Instance.GetListFood(); // Lấy danh sách món từ DAO
            this.DataContext = UserSession.CurrentUser;
        }

        #region Table Template Methods
        // Sự kiện khi click vào nút bàn
        private void TableButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            TableInfo? table = button.Tag as TableInfo;
            if (table == null) return;
            _selectedTableId = table.TableId;
            // Tìm panel thông tin trong Template
            var txtNoTableSelected = FindVisualChild<TextBlock>(MainContent, "txtNoTableSelected");
            var TableDetailPanel = FindVisualChild<StackPanel>(MainContent, "TableDetailPanel");

            var txtTableName = FindVisualChild<TextBlock>(MainContent, "txtTableName");

            var txtBillId = FindVisualChild<TextBlock>(MainContent, "txtBillId");
            var txtBillTime = FindVisualChild<TextBlock>(MainContent, "txtBillTime");

            var txtTotalAmount = FindVisualChild<TextBlock>(MainContent, "txtTotalAmount");

            // Kiểm tra null
            if (txtTableName == null) return;

            // Ẩn chữ "Chọn một bàn để xem chi tiết"
            txtNoTableSelected.Visibility = Visibility.Collapsed;
            TableDetailPanel.Visibility = Visibility.Visible;

            // ---- Gán thông tin bàn ----
            txtTableName.Text = table.TableName;
            txtBillTime.Text = BillDAO.Instance.GetTimeCheckin(_selectedTableId).ToString("g");
            _idBill = BillDAO.Instance.GetIdBillByTableID(_selectedTableId);
            txtBillId.Text = _idBill == -1 ? "N/A" : _idBill.ToString();
            statusBill = BillDAO.Instance.GetStatusBillByIdTable(_selectedTableId);
            if (statusBill == 1)
            {
                _currentOrder.Clear();
            }
            else
            {
                _currentOrder = TableDAO.Instance.GetListFoodOrderByIdTable(_selectedTableId);
                txtTotalAmount.Text = FoodDAO.Instance.FormatVND(_currentOrder.Sum(x => x.TotalPrice));
            }
            ShowTableOrder(_currentOrder);
            
        }
        private void ShowTableOrder(List<OrderItem> od)
        {
            var dgOrderItems = FindVisualChild<DataGrid>(MainContent, "dgOrderItems");
            if (dgOrderItems != null)
            {
                dgOrderItems.ItemsSource = null;
                dgOrderItems.ItemsSource = od;
            }
        }
        private void ButtonCheckout_Click(object sender, RoutedEventArgs e)
        {
            string listFoodBill = "Bàn " + _selectedTableId + "\nDanh sách món:\n";
            foreach (var item in _currentOrder)
            {
                listFoodBill += $"{item.ItemName} x{item.Quantity} - {FoodDAO.Instance.FormatVND(item.TotalPrice)}\n";
            }
            listFoodBill += $"Thành tiền: {FoodDAO.Instance.FormatVND(_currentOrder.Sum(x => x.TotalPrice))}";
            if (MessageBox.Show(listFoodBill, "Xác nhận thanh toán", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                
                TableDAO.Instance.UpdateTableStatus(_selectedTableId, "Trống");
                statusBill = 1;
                //BillDAO.Instance.ChangeStatusBill(_selectedTableId, statusBill);
                BillDAO.Instance.UpdateBill(_selectedTableId, statusBill, DateTime.Now);
                LoadTableButton();
                var txtNoTableSelected = FindVisualChild<TextBlock>(MainContent, "txtNoTableSelected");
                var TableDetailPanel = FindVisualChild<StackPanel>(MainContent, "TableDetailPanel");
                txtNoTableSelected.Visibility = Visibility.Visible;
                TableDetailPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show("Thanh toán thành công!");
            }
        }
        private void ButtonAddTable_Click(object sender, RoutedEventArgs e)
        {
            int tablenum = TableDAO.Instance.GetMaxTableID() + 1;
            string newtablename = $"Bàn {tablenum}";
            bool errorInsert = TableDAO.Instance.InsertTable(newtablename, "Trống");
            if (errorInsert)
            {
                LoadTableButton();
                return;
            }
            MessageBox.Show("Thêm thất bại");
            return;
        }

        private void ButtonDeleteTable_Click(object sender, RoutedEventArgs e)
        {
            bool errorDelete = TableDAO.Instance.DeleteTable(_selectedTableId);
            if (errorDelete)
            {
                //MessageBox.Show("Xóa thành công");
                LoadTableButton();
                return;
            }
            return;
        }

        private void ButtonEditTable_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTableId > 0)
            {
                // Hiển thị form chỉnh sửa thông tin bàn
                var table = tableList.FirstOrDefault(it => it.TableId == _selectedTableId);
                MessageBox.Show($"Chỉnh sửa thông tin Bàn {_selectedTableId}\n" +
                              $"Tên: {table.TableName}\n" +
                              $"Số ghế: {table.Seats}");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn cần chỉnh sửa!");
            }
        }
        private void LoadTableButton()
        {
            
            // 1. Lấy danh sách bàn từ SQL
            tableList = TableDAO.Instance.GetListTable();

            // 2. Tìm WrapPanel "btnGrid" bên trong MainContent
            var btnGrid = FindVisualChild<WrapPanel>(MainContent, "btnGrid");
            if (btnGrid == null)
            {
                MessageBox.Show("Không tìm thấy btnGrid trong giao diện TableTemplate!");
                return;
            }

            // Xóa nút cũ (nếu có)
            btnGrid.Children.Clear();

            // 3. Tạo button cho từng bàn
            foreach (TableInfo table in tableList)
            {
                _tables[table.TableId] = table; // Lưu trạng thái bàn vào dictionary
                // Tạo StackPanel để chứa nội dung nút
                StackPanel content = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Tên bàn
                TextBlock name = new TextBlock()
                {
                    Text = table.TableName,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Trạng thái
                TextBlock status = new TextBlock()
                {
                    Text = table.Status,
                    FontSize = 12,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                content.Children.Add(name);
                content.Children.Add(status);
                
                // Tạo button
                Button btn = new Button()
                {
                    Background = new SolidColorBrush(table.StatusColor),
                    Width = 120,
                    Height = 80,
                    Tag = table, // lưu dữ liệu bàn vào Tag
                    Content = content,
                    Margin = new Thickness(10)

                };

                // Màu theo trạng thái bàn

                btn.Click += TableButton_Click;

                // Thêm button vào WrapPanel
                btnGrid.Children.Add(btn);
            }
        }
        #endregion

        #region Menu Template Methods
        private void LoadMenuTemplate()
        {
            
            // Load danh sách món ăn vào DataGrid
            var dataMenuGrid = FindVisualChild<ItemsControl>(MainContent, "dgMenuItems");
            if (dataMenuGrid != null)
            {
                MenuItems = FoodDAO.Instance.GetListFood(); // Lấy danh sách món từ DAO
                dataMenuGrid.ItemsSource = null;               // Reset nguồn dữ liệu
                dataMenuGrid.ItemsSource = MenuItems;          // Đổ menu lên DataGrid
            }
            LoadCategoryToComboBox();
            // Reset form
            ResetMenuForm();
        }

        private void TxtSearchMenu_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox searchBox)
            {
                string searchText = searchBox.Text.ToLower();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    var dataGrid = FindName("dgMenuItems") as DataGrid;
                    if (dataGrid != null)
                    {
                        dataGrid.ItemsSource = MenuItems;
                    }
                }
                else
                {
                    var filteredItems = MenuItems.Where(item =>
                        item.Name.ToLower().Contains(searchText) ||
                        item.Category.ToLower().Contains(searchText)).ToList();

                    var dataGrid = FindName("dgMenuItems") as DataGrid;
                    if (dataGrid != null)
                    {
                        dataGrid.ItemsSource = filteredItems;
                    }
                }
            }
        }

        private void BtnAddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var nameItem = GetTextBoxText("txtMenuItemName");
            var priceText = GetTextBoxText("txtMenuItemPrice");
            var categoryItem = GetComboBoxText("cmbMenuCategory");
            if(nameItem == null)
            {
                MessageBox.Show("Vui lòng nhập tên món!");
            }
            else if(priceText == null)
            {
                MessageBox.Show("Vui lòng nhập giá hợp lệ!");
            }
            else if(categoryItem == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục món!");
            }
            else
            {
                int idcategory = FoodCategoryDAO.Instance.GetIdCategoryByName(categoryItem);
                bool error_add = FoodDAO.Instance.InsertFood(nameItem, idcategory, float.Parse(priceText.ToString()));
                if(error_add)
                {
                    // Cập nhật lại danh sách món
                    LoadMenuTemplate();
                    MessageBox.Show("Đã thêm món thành công!");
                }
                
            }
            
        }

        private void BtnEditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            int itemId = int.Parse(button.Tag.ToString()!);
            _editingMenuItem = MenuItems.FirstOrDefault(item => item.Id == itemId);
            if (_editingMenuItem == null) return;
            if (_editingMenuItem != null)
            {
                // Hiển thị thông tin món lên form
                var nameBox = FindVisualChild<TextBox>(MainContent, "txtMenuItemName");
                if (nameBox != null)
                    nameBox.Text = _editingMenuItem.Name;

                var priceBox = FindVisualChild<TextBox>(MainContent, "txtMenuItemPrice");
                if (priceBox != null)
                    priceBox.Text = _editingMenuItem.Price.ToString();

                var categoryBox = FindVisualChild<ComboBox>(MainContent, "cmbMenuCategory");
                if (categoryBox != null)
                {
                    categoryBox.SelectedItem = _editingMenuItem.Category;
                }

                // Chuyển sang chế độ chỉnh sửa
                ShowEditMode(true);
            }
        }

        private void BtnUpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (FoodDAO.Instance.UpdateFood(_editingMenuItem.Id, GetTextBoxText("txtMenuItemName"), 
                FoodCategoryDAO.Instance.GetIdCategoryByName(GetComboBoxText("cmbMenuCategory")), 
                float.Parse( GetTextBoxText("txtMenuItemPrice"))))
            {
                MessageBox.Show("Cập nhật món thành công!");
                LoadMenuTemplate();
            }
            else
            {
                MessageBox.Show("Cập nhật món thất bại!");
            }
        }

        private void BtnDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int itemId = int.Parse(button.Tag.ToString()!);
                var menuItem = MenuItems.FirstOrDefault(item => item.Id == itemId);

                if (menuItem != null)
                {
                    var result = MessageBox.Show($"Bạn có chắc muốn xóa món '{menuItem.Name}'?",
                                               "Xác nhận xóa",
                                               MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        bool error_delete = FoodDAO.Instance.DeleteFood(itemId);
                        if(error_delete)
                        {
                            LoadMenuTemplate();
                            MessageBox.Show("Đã xóa món thành công!");
                            
                        }
                    }
                }
            }
        }

        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            ResetMenuForm();
        }

        private void ShowEditMode(bool isEditing)
        {
            var addBtn = FindVisualChild<Button>(MainContent, "btnAddMenuItem");
            if (addBtn != null)
                addBtn.Visibility = isEditing ? Visibility.Collapsed : Visibility.Visible;

            var updateBtn = FindVisualChild<Button>(MainContent, "btnUpdateMenuItem");
            if (updateBtn != null)
                updateBtn.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;

            var cancelBtn = FindVisualChild<Button>(MainContent, "btnCancelEdit");
            if (cancelBtn != null)
                cancelBtn.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ResetMenuForm()
        {
            var nameBox = FindVisualChild<TextBox>(MainContent, "txtMenuItemName");
            if (nameBox != null)
                nameBox.Text = "";

            var priceBox = FindVisualChild<TextBox>(MainContent, "txtMenuItemPrice");
            if (priceBox != null)
                priceBox.Text = "";

            var categoryBox = FindVisualChild<ComboBox>(MainContent, "cmbMenuCategory");
            if (categoryBox != null)
                categoryBox.SelectedIndex = 0;

            _editingMenuItem = null;
            ShowEditMode(false);
        }
        private void LoadCategoryToComboBox()
        {
            var list = FoodCategoryDAO.Instance.GetListFoodCategory();
            var cmbTables = FindVisualChild<ComboBox>(MainContent, "cmbMenuCategory");
            cmbTables.ItemsSource = list;
        }
        #endregion

        #region Order Template Methods
        private void LoadOrderTemplate()
        { 
            var foodGrid = FindVisualChild<WrapPanel>(MainContent, "foodGrid");
            if (foodGrid == null)
            {
                MessageBox.Show("Không tìm thấy btnGrid trong giao diện TableTemplate!");
                return;
            }

            // Xóa nút cũ (nếu có)
            foodGrid.Children.Clear();

            // 3. Tạo button cho từng bàn
            foreach (FoodItem food in MenuItems)
            {
                // Tạo StackPanel để chứa nội dung nút
                StackPanel content = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Tên bàn
                TextBlock name = new TextBlock()
                {
                    Text = food.Name,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Trạng thái
                TextBlock Price = new TextBlock()
                {
                    Text = FoodDAO.Instance.FormatVND(food.Price),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                content.Children.Add(name);
                content.Children.Add(Price);

                // Tạo button
                Button btn = new Button()
                {
                    Background = new SolidColorBrush(Colors.Green),
                    Width = 120,
                    Height = 80,
                    Margin = new Thickness(10),
                    Tag = food.Id, // lưu dữ liệu bàn vào Tag
                    Content = content
                };
                btn.Click += FoodBtn_Click;

                // Thêm button vào WrapPanel
                foodGrid.Children.Add(btn);
            }
            LoadTablesToComboBox();
        }


        private void BtnSearchFood_Click(object sender, RoutedEventArgs e)
        {
            var searchBox = FindName("txtSearchFood") as TextBox;
            if (searchBox != null)
            {
                string searchText = searchBox.Text.ToLower();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    var listBox = FindName("lstMenuItems") as ListBox;
                    if (listBox != null)
                    {
                        listBox.ItemsSource = MenuItems; // DÙNG CHUNG DATA
                    }
                }
                else
                {
                    var filteredItems = MenuItems.Where(item =>  // DÙNG CHUNG DATA
                        item.Name.ToLower().Contains(searchText) ||
                        item.Category.ToLower().Contains(searchText)).ToList();

                    var listBox = FindName("lstMenuItems") as ListBox;
                    if (listBox != null)
                    {
                        listBox.ItemsSource = filteredItems;
                    }
                }
            }
        }

        private void FoodBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int foodId = int.Parse(btn.Tag.ToString());

                // Tìm món trong MenuItems
                var food = FoodDAO.Instance.GetFoodByID(foodId);
                if (food == null) return;

                // Kiểm tra xem đã có trong Order chưa
                var exist = _currentOrder.FirstOrDefault(x => x.ItemId == foodId);

                if (exist != null)
                {
                    exist.Quantity++;  // tăng số lượng
                }
                else
                {
                    _currentOrder.Add(new OrderItem
                    {
                        ItemId = food.Id,
                        ItemName = food.Name,
                        Price = food.Price,
                        Quantity = 1
                    });
                }
                UpdateOrderDisplay();
            }
        }

        private void BtnIncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int itemId = int.Parse(button.Tag.ToString()!);
                var orderItem = _currentOrder.FirstOrDefault(item => item.ItemId == itemId);

                if (orderItem != null)
                {
                    orderItem.Quantity++;
                    UpdateOrderDisplay();
                }
            }
        }

        private void BtnDecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int itemId = int.Parse(button.Tag.ToString()!);
                var orderItem = _currentOrder.FirstOrDefault(item => item.ItemId == itemId);

                if (orderItem != null)
                {
                    orderItem.Quantity--;
                    if (orderItem.Quantity <= 0)
                    {
                        _currentOrder.Remove(orderItem);
                    }
                    UpdateOrderDisplay();
                }
            }
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int itemId = int.Parse(button.Tag.ToString()!);
                var orderItem = _currentOrder.FirstOrDefault(item => item.ItemId == itemId);

                if (orderItem != null)
                {
                    _currentOrder.Remove(orderItem);
                    UpdateOrderDisplay();
                }
            }
        }

        private void BtnClearOrder_Click(object sender, RoutedEventArgs e)
        {
            _currentOrder.Clear();
            UpdateOrderDisplay();
        }

        private void BtnConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentOrder.Any())
            {
                MessageBox.Show("Vui lòng thêm món vào order trước khi xác nhận!");
                return;
            }

            var comboBox = FindVisualChild<ComboBox>(MainContent, "cmbTables");
            if (comboBox != null && comboBox.SelectedItem is TableInfo selectedTable)
            {
                bool error_createBill = BillDAO.Instance.CreateBill(selectedTable.TableId);
                _idBill = BillDAO.Instance.GetIdBillByTableID(selectedTable.TableId);
                // Thêm các món từ order vào bill
                foreach (var orderItem in _currentOrder)
                {
                    BillInfoDAO.Instance.AddBillInfo(_idBill, orderItem.ItemId, orderItem.Quantity);
                }
                // Chuyển trạng thái bàn sang "Có khách"
                //ChangeToOccupied(selectedTable.TableId);

                // Clear order
                _currentOrder.Clear();
                UpdateOrderDisplay();
                TableDAO.Instance.UpdateTableStatus(selectedTable.TableId, "Có khách");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi xác nhận order!");
            }
        }

        private void CmbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmbTables = FindVisualChild<ComboBox>(MainContent, "cmbTables");
            if (cmbTables.SelectedValue == null) return;

            _selectedTableId = (int)cmbTables.SelectedValue;
        }

        private void UpdateOrderDisplay()
        {
            var lstCurrentOrder = FindVisualChild<ListView>(MainContent, "lstCurrentOrder");
            if (lstCurrentOrder != null)
            {
                lstCurrentOrder.ItemsSource = null;
                lstCurrentOrder.ItemsSource = _currentOrder;
                var totalPrice = FindVisualChild<TextBlock>(MainContent, "txtOrderTotal");
                totalPrice.Text = FoodDAO.Instance.FormatVND(_currentOrder.Sum(x => x.Price * x.Quantity));
            }
        }
        private void LoadTablesToComboBox()
        {
            var list = TableDAO.Instance.GetListTable();
            var cmbTables = FindVisualChild<ComboBox>(MainContent, "cmbTables");
            cmbTables.ItemsSource = list;
        }
        
        #endregion

        #region Account Template Methods
        private void ButtonChangePass_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Window.GetWindow(this);
            main.LoadLayout(new ChangePasswordLayout());
        }

        private void ButtonExitAcc_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Window.GetWindow(this);
            main.LoadLayout(new LoginLayout());
        }

        #endregion

        #region Revenue Template Methods
        private void LoadRevenueTemplate()
        {
            var dgRevenue = FindVisualChild<DataGrid>(MainContent, "dgRevenueDetails");
            if (dgRevenue != null)
            {
                dgRevenue.ItemsSource = null;
                dgRevenue.ItemsSource = ListBillsReport;
            }
        }
        private void GetReport()
        {
            var startDatePicker = FindVisualChild<DatePicker>(MainContent, "dpSelectedDateStart");
            var endDatePicker = FindVisualChild<DatePicker>(MainContent, "dpSelectedDateEnd");

            if (startDatePicker == null || endDatePicker == null)
            {
                MessageBox.Show("Không tìm thấy DatePicker!");
                return;
            }

            if (!startDatePicker.SelectedDate.HasValue || !endDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ ngày bắt đầu và kết thúc!");
                return;
            }

            DateTime start = startDatePicker.SelectedDate.Value.Date;
            DateTime end = endDatePicker.SelectedDate.Value.Date;
    

            if (start > end)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                return;
            }

            ListBillsReport = BillDAO.Instance.GetListBillRange(start, end);
        }
        private void BtnViewReport_Click(object sender, RoutedEventArgs e)
        {
            GetReport();
            LoadRevenueTemplate();
        }

        private void BtnExportReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng xuất báo cáo Excel sẽ được thực hiện ở phiên bản sau!");
            // TODO: Implement Excel export functionality
        }

        // Navigation method for Revenue

        #endregion

        #region Helper Methods
        private string GetTextBoxText(string textBoxName)
        {
            var textBox = FindVisualChild<TextBox>(MainContent, textBoxName);
            return textBox?.Text ?? string.Empty;
        }

        private string GetComboBoxText(string comboBoxName)
        {
            var comboBox = FindVisualChild<ComboBox>(MainContent, comboBoxName);
            if (comboBox != null && comboBox.SelectedValue != null)
            {
                return comboBox.SelectedValue.ToString()!;
            }
            return string.Empty;
        }
        public static T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T fe && fe.Name == name)
                    return fe;

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
        #endregion

        #region Navigation Methods
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)Resources["AccountTemplate"];
        }

        private void TableNavButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)Resources["TableTemplate"];

            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadTableButton();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)Resources["MenuTemplate"];
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadMenuTemplate(); // Load dữ liệu khi chuyển sang Menu
            }), System.Windows.Threading.DispatcherPriority.Background);

        }
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)Resources["OrderTemplate"];
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadOrderTemplate(); // Load dữ liệu khi chuyển sang Order
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        private void RevenueButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)Resources["RevenueTemplate"];
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadRevenueTemplate(); // Load dữ liệu khi chuyển sang Revenue
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        #endregion
    }
}              