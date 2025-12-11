using cafe_management.DAO;
using cafe_management.DTO;  
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
        public ObservableCollection<Bill> Bills { get; set; } = new ObservableCollection<Bill>();
        private int _selectedTableId = 0;
        public List<TableInfo> tableList { get; set; } = new List<TableInfo>();

        // Dictionary để lưu trạng thái các bàn
        private Dictionary<int, TableInfo> _tables = new Dictionary<int, TableInfo>();

        // Danh sách menu đồ ăn - DÙNG CHUNG cho cả MenuTemplate và OrderTemplate
        public List<FoodItem> MenuItems { get; set; } = new List<FoodItem>();

        // Order hiện tại
        private ObservableCollection<OrderItem> _currentOrder = new ObservableCollection<OrderItem>();

        // Biến tạm để lưu món đang chỉnh sửa
        private FoodItem? _editingMenuItem = null;

        private int _idBill = 0;
        private int _tableNumber = 1;
        public HomeLayout()
        {
            InitializeComponent();
            MenuItems = FoodDAO.Instance.GetListFood(); // Lấy danh sách món từ DAO
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
            txtBillTime.Text = BillDAO.Instance.GetTimeCheckin(table.TableId).ToString("g");
            var dgOrderItems = FindVisualChild<DataGrid>(MainContent, "dgOrderItems");
            if (dgOrderItems != null)
            {
                
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
                    Margin = new Thickness(10),
                    Tag = table, // lưu dữ liệu bàn vào Tag
                    Content = content
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
            var dataMenuGrid = FindVisualChild<DataGrid>(MainContent, "dgMenuItems");
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
                        bool error_delete = FoodDAO.Instance.DeleteFood(itemId);
                        if(error_delete)
                        {
                            MessageBox.Show("Đã xóa món thành công!");
                            LoadMenuTemplate();
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
                TableDAO.Instance.UpdateTableStatus(selectedTable.TableId, "có khách");
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
        #endregion

        #region Other Events
        private void ButtonAddTable_Click(object sender, RoutedEventArgs e)
        {
            string newtablename = $"Bàn {_tableNumber}";
            bool errorInsert = TableDAO.Instance.InsertTable(newtablename, "Trống");
            if (errorInsert)
            {
                LoadTableButton();
                _tableNumber++;
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
            MainWindow main = (MainWindow)Window.GetWindow(this);
            main.LoadLayout(new ChangePasswordLayout());
        }

        private void ButtonExitAcc_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Window.GetWindow(this);
            main.LoadLayout(new LoginLayout());
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
}