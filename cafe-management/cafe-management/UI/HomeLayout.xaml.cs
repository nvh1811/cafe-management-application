using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;
using System;
using System.Collections.Generic;

namespace cafe_management.UI
{
    public partial class HomeLayout : UserControl
    {
        public ObservableCollection<Bill> Bills { get; set; } = new ObservableCollection<Bill>();
        private int _selectedTableId = 0;

        // Dictionary để lưu trạng thái các bàn
        private Dictionary<int, TableInfo> _tables = new Dictionary<int, TableInfo>();

        // Danh sách menu đồ ăn - DÙNG CHUNG cho cả MenuTemplate và OrderTemplate
        public ObservableCollection<FoodItem> MenuItems { get; set; } = new ObservableCollection<FoodItem>();

        // Order hiện tại
        private ObservableCollection<OrderItem> _currentOrder = new ObservableCollection<OrderItem>();

        // Biến tạm để lưu món đang chỉnh sửa
        private FoodItem? _editingMenuItem = null;

        public HomeLayout()
        {
            InitializeComponent();
            InitializeTables();
            InitializeBills();
            InitializeMenu();

            // Đảm bảo hiển thị TableTemplate khi khởi động
            MainContent.ContentTemplate = (DataTemplate)Resources["TableTemplate"];
        }

        private void InitializeTables()
        {
            // Khởi tạo thông tin 6 bàn
            _tables.Clear();
            _tables.Add(1, new TableInfo { TableId = 1, TableName = "Bàn 1", Status = "Trống", Seats = 4, StatusColor = Colors.Green });
            _tables.Add(2, new TableInfo { TableId = 2, TableName = "Bàn 2", Status = "Có khách", Seats = 2, StatusColor = Colors.Orange });
            _tables.Add(3, new TableInfo { TableId = 3, TableName = "Bàn 3", Status = "Trống", Seats = 6, StatusColor = Colors.Green });
            _tables.Add(4, new TableInfo { TableId = 4, TableName = "Bàn 4", Status = "Đặt trước", Seats = 4, StatusColor = Colors.Yellow });
            _tables.Add(5, new TableInfo { TableId = 5, TableName = "Bàn 5", Status = "Có khách", Seats = 8, StatusColor = Colors.Orange });
            _tables.Add(6, new TableInfo { TableId = 6, TableName = "Bàn 6", Status = "Trống", Seats = 4, StatusColor = Colors.Green });
        }

        private void InitializeMenu()
        {
            MenuItems.Clear();

            // Thêm các món đồ uống
            MenuItems.Add(new FoodItem { Id = 1, Name = "Cà phê đen", Price = 20000, Category = "Đồ uống" });
            MenuItems.Add(new FoodItem { Id = 2, Name = "Cà phê sữa", Price = 25000, Category = "Đồ uống" });
            MenuItems.Add(new FoodItem { Id = 3, Name = "Trà đá", Price = 10000, Category = "Đồ uống" });
            MenuItems.Add(new FoodItem { Id = 4, Name = "Trà sữa", Price = 30000, Category = "Đồ uống" });
            MenuItems.Add(new FoodItem { Id = 5, Name = "Nước cam", Price = 25000, Category = "Đồ uống" });
            MenuItems.Add(new FoodItem { Id = 6, Name = "Sinh tố xoài", Price = 35000, Category = "Đồ uống" });

            // Thêm các món đồ ăn            
            MenuItems.Add(new FoodItem { Id = 9, Name = "Bánh ngọt", Price = 12000, Category = "Đồ ăn" });
            MenuItems.Add(new FoodItem { Id = 10, Name = "Bim bim", Price = 10000, Category = "Đồ ăn" });
            MenuItems.Add(new FoodItem { Id = 11, Name = "Mì tôm", Price = 15000, Category = "Đồ ăn" });
            MenuItems.Add(new FoodItem { Id = 12, Name = "Xôi", Price = 20000, Category = "Đồ ăn" });
        }

        private void InitializeBills()
        {
            Bills.Clear();
            // Bill cho bàn 2
            Bills.Add(new Bill
            {
                BillId = "HD001",
                TableId = 2,
                StartTime = DateTime.Now.AddHours(-1),
                Items = new ObservableCollection<BillItem>
                {
                    new BillItem { ItemName = "Cà phê đen", Quantity = 2, Price = 20000 },
                    new BillItem { ItemName = "Bánh ngọt", Quantity = 1, Price = 15000 }
                }
            });
            // Bill cho bàn 5
            Bills.Add(new Bill
            {
                BillId = "HD002",
                TableId = 5,
                StartTime = DateTime.Now.AddMinutes(-30),
                Items = new ObservableCollection<BillItem>
                {
                    new BillItem { ItemName = "Cà phê sữa", Quantity = 3, Price = 25000 },
                    new BillItem { ItemName = "Bánh ngọt", Quantity = 2, Price = 12000 }
                }
            });
        }

        #region Table Template Methods
        // Sự kiện khi click vào nút bàn
        private void TableButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int tableId))
            {
                _selectedTableId = tableId;
                ShowTableDetails(tableId);
            }
        }

        private void ShowTableDetails(int tableId)
        {
            if (_tables.ContainsKey(tableId))
            {
                var table = _tables[tableId];

                // Hiển thị thông tin chi tiết
                MessageBox.Show($"Bàn: {table.TableName}\n" +
                              $"Trạng thái: {table.Status}\n" +
                              $"Số ghế: {table.Seats}\n" +
                              $"ID: {table.TableId}",
                              "Thông Tin Bàn");

                // Hiển thị menu thay đổi trạng thái
                ShowStatusChangeMenu(tableId);
            }
        }

        private void ShowStatusChangeMenu(int tableId)
        {
            var table = _tables[tableId];

            // Tạo context menu để thay đổi trạng thái
            ContextMenu menu = new ContextMenu();

            System.Windows.Controls.MenuItem menuItem1 = new System.Windows.Controls.MenuItem();
            menuItem1.Header = "Chuyển sang TRỐNG";
            menuItem1.Click += (s, e) => ChangeTableStatus(tableId, "Trống", Colors.Green);
            menu.Items.Add(menuItem1);

            System.Windows.Controls.MenuItem menuItem2 = new System.Windows.Controls.MenuItem();
            menuItem2.Header = "Chuyển sang CÓ KHÁCH";
            menuItem2.Click += (s, e) => ChangeTableStatus(tableId, "Có khách", Colors.Orange);
            menu.Items.Add(menuItem2);

            System.Windows.Controls.MenuItem menuItem3 = new System.Windows.Controls.MenuItem();
            menuItem3.Header = "Chuyển sang ĐẶT TRƯỚC";
            menuItem3.Click += (s, e) => ChangeTableStatus(tableId, "Đặt trước", Colors.Yellow);
            menu.Items.Add(menuItem3);

            // Hiển thị menu
            var button = FindName($"btnTable{tableId}") as Button;
            if (button != null)
            {
                menu.PlacementTarget = button;
                menu.IsOpen = true;
            }
        }

        private void ChangeTableStatus(int tableId, string newStatus, Color newColor)
        {
            if (_tables.ContainsKey(tableId))
            {
                // Cập nhật trạng thái
                _tables[tableId].Status = newStatus;
                _tables[tableId].StatusColor = newColor;

                // Cập nhật giao diện
                UpdateTableButton(tableId);

                MessageBox.Show($"Đã chuyển {_tables[tableId].TableName} sang trạng thái: {newStatus}");
            }
        }

        private void UpdateTableButton(int tableId)
        {
            // Tìm button tương ứng và cập nhật - XỬ LÝ NULL AN TOÀN
            var table = _tables[tableId];
            var buttonName = $"btnTable{tableId}";
            var button = FindName(buttonName) as Button;

            if (button != null)
            {
                // Cập nhật màu nền
                button.Background = new SolidColorBrush(table.StatusColor);

                // Cập nhật text status - XỬ LÝ NULL AN TOÀN
                if (button.Content is StackPanel stackPanel && stackPanel.Children.Count > 1)
                {
                    if (stackPanel.Children[1] is TextBlock statusTextBlock)
                    {
                        statusTextBlock.Text = table.Status;
                    }
                }
            }
        }

        // Các phương thức chuyển trạng thái nhanh
        private void ChangeToAvailable(int tableId)
        {
            ChangeTableStatus(tableId, "Trống", Colors.Green);
        }

        private void ChangeToOccupied(int tableId)
        {
            ChangeTableStatus(tableId, "Có khách", Colors.Orange);

            // Tự động tạo bill nếu chưa có
            if (!Bills.Any(b => b.TableId == tableId))
            {
                var newBill = new Bill
                {
                    BillId = $"HD{(Bills.Count + 1):D3}",
                    TableId = tableId,
                    StartTime = DateTime.Now,
                    Items = new ObservableCollection<BillItem>()
                };
                Bills.Add(newBill);
            }
        }

        private void ChangeToReserved(int tableId)
        {
            ChangeTableStatus(tableId, "Đặt trước", Colors.Yellow);
        }

        private void BtnCreateBill_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTableId > 0)
            {
                var newBill = new Bill
                {
                    BillId = $"HD{(Bills.Count + 1):D3}",
                    TableId = _selectedTableId,
                    StartTime = DateTime.Now,
                    Items = new ObservableCollection<BillItem>()
                };

                Bills.Add(newBill);

                // Chuyển trạng thái bàn sang "Có khách"
                ChangeToOccupied(_selectedTableId);

                MessageBox.Show($"Đã tạo hóa đơn {newBill.BillId} cho Bàn {_selectedTableId}");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi tạo hóa đơn!");
            }
        }

        private void BtnCloseBill_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTableId > 0)
            {
                var currentBill = Bills.FirstOrDefault(b => b.TableId == _selectedTableId);
                if (currentBill != null)
                {
                    Bills.Remove(currentBill);

                    // Chuyển trạng thái bàn sang "Trống"
                    ChangeToAvailable(_selectedTableId);

                    MessageBox.Show($"Đã thanh toán {currentBill.BillId}\nTổng tiền: {currentBill.TotalAmount:N0}đ");
                }
                else
                {
                    MessageBox.Show("Bàn này không có hóa đơn để thanh toán!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi thanh toán!");
            }
        }
        #endregion

        #region Menu Template Methods
        private void LoadMenuTemplate()
        {
            // Load danh sách món ăn vào DataGrid
            var dataGrid = FindName("dgMenuItems") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = MenuItems;
            }

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
            if (ValidateMenuForm())
            {
                var newId = MenuItems.Any() ? MenuItems.Max(item => item.Id) + 1 : 1;

                var newMenuItem = new FoodItem
                {
                    Id = newId,
                    Name = GetTextBoxText("txtMenuItemName"),
                    Price = decimal.Parse(GetTextBoxText("txtMenuItemPrice")),
                    Category = GetComboBoxText("cmbMenuCategory")
                };

                MenuItems.Add(newMenuItem);
                ResetMenuForm();
                MessageBox.Show("Đã thêm món mới thành công!");
            }
        }

        private void BtnEditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int itemId = int.Parse(button.Tag.ToString()!);
                _editingMenuItem = MenuItems.FirstOrDefault(item => item.Id == itemId);

                if (_editingMenuItem != null)
                {
                    // Hiển thị thông tin món lên form
                    var nameBox = FindName("txtMenuItemName") as TextBox;
                    if (nameBox != null)
                        nameBox.Text = _editingMenuItem.Name;

                    var priceBox = FindName("txtMenuItemPrice") as TextBox;
                    if (priceBox != null)
                        priceBox.Text = _editingMenuItem.Price.ToString();

                    var categoryBox = FindName("cmbMenuCategory") as ComboBox;
                    if (categoryBox != null)
                    {
                        foreach (ComboBoxItem item in categoryBox.Items)
                        {
                            if (item.Content?.ToString() == _editingMenuItem.Category)
                            {
                                categoryBox.SelectedItem = item;
                                break;
                            }
                        }
                    }

                    // Chuyển sang chế độ chỉnh sửa
                    ShowEditMode(true);
                }
            }
        }

        private void BtnUpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_editingMenuItem != null && ValidateMenuForm())
            {
                _editingMenuItem.Name = GetTextBoxText("txtMenuItemName");
                _editingMenuItem.Price = decimal.Parse(GetTextBoxText("txtMenuItemPrice"));
                _editingMenuItem.Category = GetComboBoxText("cmbMenuCategory");

                // Refresh DataGrid
                var dataGrid = FindName("dgMenuItems") as DataGrid;
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = MenuItems;
                }

                ResetMenuForm();
                MessageBox.Show("Đã cập nhật món thành công!");
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
                        MenuItems.Remove(menuItem);
                        MessageBox.Show("Đã xóa món thành công!");
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
            var addBtn = FindName("btnAddMenuItem") as Button;
            if (addBtn != null)
                addBtn.Visibility = isEditing ? Visibility.Collapsed : Visibility.Visible;

            var updateBtn = FindName("btnUpdateMenuItem") as Button;
            if (updateBtn != null)
                updateBtn.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;

            var cancelBtn = FindName("btnCancelEdit") as Button;
            if (cancelBtn != null)
                cancelBtn.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ResetMenuForm()
        {
            var nameBox = FindName("txtMenuItemName") as TextBox;
            if (nameBox != null)
                nameBox.Text = "";

            var priceBox = FindName("txtMenuItemPrice") as TextBox;
            if (priceBox != null)
                priceBox.Text = "";

            var categoryBox = FindName("cmbMenuCategory") as ComboBox;
            if (categoryBox != null)
                categoryBox.SelectedIndex = 0;

            _editingMenuItem = null;
            ShowEditMode(false);
        }

        private bool ValidateMenuForm()
        {
            string name = GetTextBoxText("txtMenuItemName");
            string priceText = GetTextBoxText("txtMenuItemPrice");

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập tên món!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(priceText) || !decimal.TryParse(priceText, out decimal price) || price <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá hợp lệ!");
                return false;
            }

            return true;
        }
        #endregion

        #region Order Template Methods
        private void LoadOrderTemplate()
        {
            // Load danh sách món ăn từ MenuItems (DÙNG CHUNG DATA)
            var listBox = FindName("lstMenuItems") as ListBox;
            if (listBox != null)
            {
                listBox.ItemsSource = MenuItems;
            }

            // Load danh sách bàn trống vào combobox
            var comboBox = FindName("cmbTables") as ComboBox;
            if (comboBox != null)
            {
                var availableTables = _tables.Values.Where(t => t.Status == "Trống").ToList();
                comboBox.ItemsSource = availableTables;

                if (availableTables.Any())
                {
                    comboBox.SelectedIndex = 0;
                }
            }

            // Clear order hiện tại
            _currentOrder.Clear();
            UpdateOrderDisplay();
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

        private void BtnAddToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int itemId = int.Parse(button.Tag.ToString()!);
                var menuItem = MenuItems.FirstOrDefault(item => item.Id == itemId);

                if (menuItem != null)
                {
                    // Kiểm tra xem món đã có trong order chưa
                    var existingItem = _currentOrder.FirstOrDefault(item => item.ItemId == itemId);

                    if (existingItem != null)
                    {
                        // Tăng số lượng nếu đã có
                        existingItem.Quantity++;
                    }
                    else
                    {
                        // Thêm mới vào order
                        _currentOrder.Add(new OrderItem
                        {
                            ItemId = menuItem.Id,
                            ItemName = menuItem.Name,
                            Price = menuItem.Price,
                            Quantity = 1
                        });
                    }

                    UpdateOrderDisplay();
                }
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

            var comboBox = FindName("cmbTables") as ComboBox;
            if (comboBox != null && comboBox.SelectedItem is TableInfo selectedTable)
            {
                // Tạo bill mới
                var newBill = new Bill
                {
                    BillId = $"HD{(Bills.Count + 1):D3}",
                    TableId = selectedTable.TableId,
                    StartTime = DateTime.Now,
                    Items = new ObservableCollection<BillItem>()
                };

                // Thêm các món từ order vào bill
                foreach (var orderItem in _currentOrder)
                {
                    newBill.Items.Add(new BillItem
                    {
                        ItemName = orderItem.ItemName,
                        Quantity = orderItem.Quantity,
                        Price = orderItem.Price
                    });
                }

                Bills.Add(newBill);

                // Chuyển trạng thái bàn sang "Có khách"
                ChangeToOccupied(selectedTable.TableId);

                // Clear order
                _currentOrder.Clear();
                UpdateOrderDisplay();

                MessageBox.Show($"Đã tạo order cho {selectedTable.TableName}\n" +
                              $"Mã hóa đơn: {newBill.BillId}\n" +
                              $"Tổng tiền: {newBill.TotalAmount:N0}đ");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi xác nhận order!");
            }
        }

        private void CmbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Có thể thêm xử lý khi chọn bàn khác
        }

        private void UpdateOrderDisplay()
        {
            // Cập nhật ListView order
            var listView = FindName("lstCurrentOrder") as ListView;
            if (listView != null)
            {
                listView.ItemsSource = null;
                listView.ItemsSource = _currentOrder;
            }

            // Cập nhật tổng tiền
            decimal total = _currentOrder.Sum(item => item.Price * item.Quantity);
            var totalText = FindName("txtOrderTotal") as TextBlock;
            if (totalText != null)
            {
                totalText.Text = $"{total:N0}đ";
            }
        }
        #endregion

        #region Helper Methods
        private string GetTextBoxText(string textBoxName)
        {
            var textBox = FindName(textBoxName) as TextBox;
            return textBox?.Text ?? string.Empty;
        }

        private string GetComboBoxText(string comboBoxName)
        {
            var comboBox = FindName(comboBoxName) as ComboBox;
            if (comboBox?.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? string.Empty;
            }
            return string.Empty;
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
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)Resources["MenuTemplate"];
            LoadMenuTemplate(); // Load dữ liệu khi chuyển sang Menu
        }

        private void BillButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.ContentTemplate = (DataTemplate)Resources["OrderTemplate"];
            LoadOrderTemplate(); // Load dữ liệu khi chuyển sang Order
        }
        #endregion

        #region Other Events
        private void ButtonAddTable_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng thêm bàn");
        }

        private void ButtonDeleteTable_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTableId > 0)
            {
                var tableName = _tables.ContainsKey(_selectedTableId) ? _tables[_selectedTableId].TableName : $"Bàn {_selectedTableId}";
                MessageBox.Show($"Đã xóa {tableName}");
                _selectedTableId = 0;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn cần xóa!");
            }
        }

        private void ButtonEditTable_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTableId > 0 && _tables.ContainsKey(_selectedTableId))
            {
                // Hiển thị form chỉnh sửa thông tin bàn
                var table = _tables[_selectedTableId];
                MessageBox.Show($"Chỉnh sửa thông tin Bàn {_selectedTableId}\n" +
                              $"Tên: {table.TableName}\n" +
                              $"Số ghế: {table.Seats}");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn cần chỉnh sửa!");
            }
        }

        private void ButtonChangePass_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng đổi mật khẩu");
        }

        private void ButtonExitAcc_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đăng xuất");
        }

        private void ButtonAddFood_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thêm món ăn - Sử dụng Menu Management để thêm món");
        }

        private void ButtonDeleteFood_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Xóa món ăn - Sử dụng Menu Management để xóa món");
        }
        #endregion
    }

    #region Support Classes
    // Class lưu thông tin bàn
    public class TableInfo
    {
        public int TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Status { get; set; } = "Trống";
        public int Seats { get; set; } = 4;
        public Color StatusColor { get; set; } = Colors.Green;
    }

    public class Bill
    {
        public string BillId { get; set; } = string.Empty;
        public int TableId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public ObservableCollection<BillItem> Items { get; set; } = new ObservableCollection<BillItem>();

        public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    }

    public class BillItem
    {
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }

    // ĐỔI TÊN từ MenuItem thành FoodItem để tránh xung đột
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    // Class mới cho Order Item
    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    #endregion
}