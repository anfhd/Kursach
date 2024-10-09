using Caffee.DALs;
using Caffee.Models;
using Microsoft.Data.SqlClient;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using RestaurantAPI.Dal;
using System.Windows;
using System.Windows.Controls;
using OxyPlot.WindowsForms;
using System.Linq;
using System;

namespace Caffee.UI
{
    public enum Role
    {
        Guest,
        Visitor,
        Employee
    }
    public partial class MainWindow : System.Windows.Window
    {
        private Order _currentOrder = new Order();
        private Visitor _currentVisitor = new Visitor();
        private Waiter _currentWaiter = new Waiter();

        private Role _role = Role.Guest;

        private CategoryDAL _categoryService;
        private DishDAL _dishService;
        private DiscountDAL _discountService;
        private OrderDAL _orderService;
        private OrderDetailDAL _orderDetailService;
        private VisitorDAL _visitorService;
        private StatusDAL _statusService;

        private void SetMock()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = "MRAKIV",
                InitialCatalog = "Caffee",
                TrustServerCertificate = true,
                IntegratedSecurity = true,
                MultipleActiveResultSets = true,
                UserID = "guest",
                Password = "1111",
            };

            string connectionString = sqlConnectionStringBuilder.ConnectionString;

            _categoryService = new CategoryDAL(connectionString);
            _dishService = new DishDAL(connectionString);
            _discountService = new DiscountDAL(connectionString);
            _orderService = new OrderDAL(connectionString);
            _orderDetailService = new OrderDetailDAL(connectionString);
            _visitorService = new VisitorDAL(connectionString);
            _statusService = new StatusDAL(connectionString);

            _currentVisitor = _visitorService.GetAll().First();
            _currentWaiter = _visitorService.GetAllWaiters().First();
            _role = Role.Guest;

            _currentOrder.OrderDetails = new List<OrderDetail>();
        }

        public MainWindow()
        {
            InitializeComponent();
            SetMock();
            SetLogGrid();
            LogIn();
        }

        private void LogIn()
        {
            Login login = new Login();
            login.ShowDialog();

            if (login.Success == true)
            {
                _role = login.Role;

                if(_role == Role.Visitor)
                {
                    _currentVisitor = _visitorService.GetAll().First();
                }

                this.LogInGrid.Visibility = Visibility.Hidden;
                this.LogOutGrid.Visibility = Visibility.Visible;
            }
            else
            {
                _role = Role.Guest;
                this.LogInGrid.Visibility = Visibility.Visible;
                this.LogOutGrid.Visibility = Visibility.Hidden;
            }

            SetMenu();
        }

        private void SetCreateOrderPanelVisible(bool visible)
        {
            this.CreateOrderGrid.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
        private void SetMenuPanelVisible(bool visible)
        {
            this.MenuGrid.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
        private void SetDiscountsPanelVisible(bool visible)
        {
            this.DiscountsGrid.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
        private void SetOrdersPanelVisible(bool visible)
        {
            this.OrdersGrid.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }


        private void SetMenu()
        {
            switch(_role)
            {
                case Role.Guest:
                    this.CreateOrderButton.IsEnabled = false;
                    this.OrdersButton.IsEnabled = false;
                    this.MenuButton.IsEnabled = true;
                    this.DiscountButton.IsEnabled = true;
                    this.StatisticButton.IsEnabled = false;

                    this.LogOutGrid.Visibility = Visibility.Hidden;
                    this.LogInGrid.Visibility = Visibility.Visible;
                    break;

                case Role.Visitor:
                    this.CreateOrderButton.IsEnabled = true;
                    this.OrdersButton.IsEnabled = true;
                    this.MenuButton.IsEnabled = true;
                    this.DiscountButton.IsEnabled = true;
                    this.StatisticButton.IsEnabled = false;

                    this.LogOutGrid.Visibility = Visibility.Visible;
                    this.LogInGrid.Visibility = Visibility.Hidden;
                    break;

                case Role.Employee:
                    this.CreateOrderButton.IsEnabled = false;
                    this.OrdersButton.IsEnabled = true;
                    this.MenuButton.IsEnabled = true;
                    this.DiscountButton.IsEnabled = true;
                    this.StatisticButton.IsEnabled = true;

                    this.LogOutGrid.Visibility = Visibility.Visible;
                    this.LogInGrid.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void SetLogGrid()
        {
            if (_currentVisitor != null)
            {
                this.LogOutGrid.Visibility = Visibility.Visible;
            }
        }
        private void SetMenuPanel()
        {
            SetCreateOrderPanelVisible(false);
            SetDiscountsPanelVisible(false);
            SetOrdersPanelVisible(false);
            SetStaticticGridVisibility(false);

            ClearMenuPanel();
            SetMenuPanelVisible(true);
            LoadMenuTabControlWithData();
        }
        private void SetCreateOrderPanel()
        {
            SetMenuPanelVisible(false);
            SetDiscountsPanelVisible(false);
            SetOrdersPanelVisible(false);
            SetStaticticGridVisibility(false);

            ClearCreateOrderPanel();
            SetCreateOrderPanelVisible(true);

            LoadOrderDetailPanelWithData();
        }
        private void SetDiscountPanel()
        {
            SetCreateOrderPanelVisible(false);
            SetMenuPanelVisible(false);
            SetOrdersPanelVisible(false);
            SetStaticticGridVisibility(false);

            SetDiscountsPanelVisible(true);

            LoadDiscountTableWithData();
        }
        private void SetOrdersPanel()
        {
            SetCreateOrderPanelVisible(false);
            SetMenuPanelVisible(false);
            SetDiscountsPanelVisible(false);
            SetStaticticGridVisibility(false);

            SetOrdersPanelVisible(true);

            ClearOrdersPanel();
            LoadOrdersWithData();
        }

        private void ClearOrdersPanel()
        {
            this.OrdersTableGrid.Children.Clear();

            int max = 0;
            if (_role == Role.Visitor) max = 4;
            else max = 450;

            for (int i = 0; i < max; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(140);
                this.OrdersTableGrid.RowDefinitions.Add(rowDefinition);
            }

            // Додавання ColumnDefinitions
            for (int i = 0; i < 5; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(140);
                this.OrdersTableGrid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private void RemoveOrderDetailFromOrder(OrderDetail orderDetail)
        {
            _currentOrder.OrderDetails = _currentOrder.OrderDetails.Where(x => x.ID != orderDetail.ID).ToList();
            SetCreateOrderPanel();
        }
        private void LoadOrderDetailPanelWithData()
        {
            foreach (var item in _currentOrder.OrderDetails)
            {
                var grid = new Grid();
                grid.Background = System.Windows.Media.Brushes.WhiteSmoke;
                grid.Height = 60;

                var dishLabel = new System.Windows.Controls.Label();
                dishLabel.Content = item.Dish.Name;
                dishLabel.Margin = new Thickness(30, 15, 398, 15);
                dishLabel.Height = 30;
                grid.Children.Add(dishLabel);

                var amountTextBox = new System.Windows.Controls.TextBox();
                amountTextBox.Text = item.Amount.ToString();
                amountTextBox.Margin = new Thickness(332, 15, 282, 15);
                amountTextBox.TextChanged += (sender, e) =>
                {
                    if (int.TryParse(amountTextBox.Text, out var amount) is true)
                        AddDishToOrder(item.Dish, int.Parse(amountTextBox.Text));
                };
                grid.Children.Add(amountTextBox);

                var unitPriceLabel = new System.Windows.Controls.Label();
                unitPriceLabel.Content = item.UnitPrice.ToString();
                unitPriceLabel.Margin = new Thickness(424, 15, 193, 15);
                grid.Children.Add(unitPriceLabel);

                var priceLabel = new System.Windows.Controls.Label();
                priceLabel.Content = (item.UnitPrice * item.Amount).ToString();
                priceLabel.Margin = new Thickness(511, 15, 102, 15);
                grid.Children.Add(priceLabel);

                var deleteButton = new System.Windows.Controls.Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += (sender, e) =>
                {
                    RemoveOrderDetailFromOrder(item);
                };
                deleteButton.Margin = new Thickness(615, 15, 10, 15);
                grid.Children.Add(deleteButton);

                Border border = new();
                border.BorderThickness = new System.Windows.Thickness(1);
                border.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                border.Child = grid;

                this.OrderDetailsStackPanel.Children.Add(border);
            }

            this.DiscountLabel.Content = _currentOrder.Discount is null ? "None :(" : _currentOrder.Discount.Type.Type.Contains("per") ? $"{_currentOrder.Discount.Value}%" : $"{_currentOrder.Discount.Value}$";
        }

        private void ClearCreateOrderPanel()
        {
            this.OrderDetailsStackPanel.Children.Clear();
        }

        private void LoadMenuTabControlWithData(bool filtered = false)
        {
            var categories = _categoryService.GetAll();
            var dishes = _dishService.GetAll();

            if (filtered == true)
            {
                if (this.MinTextBox.Text != "")
                {
                    double.Parse(this.MinTextBox.Text);
                    dishes = dishes.Where(x => x.Price >= double.Parse(this.MinTextBox.Text) && x.Price <= double.Parse(this.MaxTextBox.Text)).ToList();
                }
                dishes = dishes.Where(x => x.Name.Contains(this.NameTextBox.Text)).ToList();
            }

            var dishesDictionary = dishes.Where(x => categories.Any(y => x.Category.ID == y.ID)).ToList().GroupBy(x => x.Category.ID).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var category in categories)
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = category.Name;

                Grid oouterGrid = new Grid();
                tabItem.Content = oouterGrid;

                ScrollViewer scrollViewer = new ScrollViewer();
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                oouterGrid.Children.Add(scrollViewer);

                Grid outerGrid = new Grid();
                scrollViewer.Content = outerGrid;
                int margin = 0;

                this.MenuTabControl.Items.Add(tabItem);

                if (dishesDictionary.Keys.Contains(category.ID) is false) continue;

                foreach (var dish in dishesDictionary[category.ID])
                {
                    Grid grid = new Grid();
                    grid.Margin = new Thickness(26, 0 + margin, 23, 535 - margin);
                    grid.Background = System.Windows.Media.Brushes.WhiteSmoke;

                    margin += 170;

                    System.Windows.Controls.Label dishLabel = new System.Windows.Controls.Label();
                    dishLabel.Content = dish.Name;
                    dishLabel.Margin = new Thickness(10, 10, 10, 10);
                    dishLabel.FontSize = 16;
                    dishLabel.Foreground = System.Windows.Media.Brushes.Black;

                    System.Windows.Controls.Label priceLabel = new System.Windows.Controls.Label();
                    priceLabel.Content = dish.Price.ToString() + "₴";
                    priceLabel.Margin = new Thickness(10, 35, 10, 10);
                    priceLabel.FontSize = 14;
                    priceLabel.Foreground = System.Windows.Media.Brushes.DarkGreen;

                    System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
                    textBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    textBox.Margin = new Thickness(10, 60, 10, 10);
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.Text = dish.Description;
                    textBox.Background = System.Windows.Media.Brushes.LightGray;
                    textBox.FontSize = 14;
                    textBox.Width = 650;
                    textBox.Height = 50;


                    System.Windows.Controls.Button addButton = new System.Windows.Controls.Button();
                    addButton.Click += (sender, e) =>
                    {
                        AddDishToOrder(dish);
                    };
                    addButton.Content = "Add";
                    addButton.Width = 100;
                    addButton.Margin = new Thickness(10, 140, 10, 10);
                    addButton.Background = System.Windows.Media.Brushes.Azure;
                    addButton.Foreground = System.Windows.Media.Brushes.DarkBlue;

                    grid.Children.Add(dishLabel);
                    grid.Children.Add(priceLabel);
                    grid.Children.Add(textBox);
                    grid.Children.Add(addButton);

                    outerGrid.Children.Add(grid);

                }

                
            }
        }
        private void AddDishToOrder(Dish dish, int amount = -1)
        {
            if (_currentOrder.OrderDetails.Any(x => x.ID == dish.ID))
            {
                var currentDish = _currentOrder.OrderDetails.Where(x => x.ID == dish.ID).First();
                if (amount == -1)
                {
                    currentDish.Amount++;
                }
                else currentDish.Amount = amount;
            }
            else
            {
                _currentOrder.OrderDetails.Add(new OrderDetail()
                {
                    ID = dish.ID,
                    Dish = dish,
                    Amount = 1,
                    UnitPrice = dish.Price
                });
            }
        }
        private void ClearMenuPanel(bool leftTextBoxes = false)
        {
            this.MenuTabControl.Items.Clear();
            if (leftTextBoxes == false)
            {
                this.MinTextBox.Text = string.Empty;
                this.MaxTextBox.Text = string.Empty;
                this.NameTextBox.Text = string.Empty;
            }
        }

        private void LoadOrdersWithData()
        {
            List<Order> orders;

            if (_role == Role.Visitor) orders = _orderService.GetAll(_currentVisitor, _orderDetailService, _dishService).OrderByDescending(x => x.CreationTime).ToList();
            else orders = _orderService.GetAll(_orderDetailService, _dishService).OrderByDescending(x => x.CreationTime).ToList();

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (orders.Count == 0) continue;
                    var grid = GetOrderGrid(orders.First());

                    Border border = new Border();
                    border.BorderThickness = new Thickness(3);
                    border.BorderBrush = System.Windows.Media.Brushes.LightGray;

                    border.Child = grid;

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    this.OrdersTableGrid.Children.Add(border);
                    orders.RemoveAt(0);
                }
            }
        }


        private Grid GetOrderGrid(Order order)
        {
            Grid grid = new Grid();

            var statuses = _statusService.GetAll();

            if (_role == Role.Employee)
            {
                ContextMenu contextMenu = new ContextMenu();

                foreach (var status in statuses)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Change to " + status.Name;
                    menuItem.Click += (sender, e) =>
                    {
                        _orderService.ChangeStatus(order, status);
                        SetOrdersPanel();
                    };

                    contextMenu.Items.Add(menuItem);
                }

                grid.ContextMenu = contextMenu;
            }

            grid.Background = System.Windows.Media.Brushes.WhiteSmoke;
            System.Windows.Controls.Label label1 = new System.Windows.Controls.Label();
            label1.Content = order.ID.ToString();
            label1.Margin = new Thickness(0, 0, 0, 99);
            grid.Children.Add(label1);

            System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
            label2.Content = "Table";
            label2.Margin = new Thickness(0, 22, 0, 67);
            grid.Children.Add(label2);

            System.Windows.Controls.Label label3 = new System.Windows.Controls.Label();
            label3.Content = $"No. {order.Table.ID}";
            label3.Margin = new Thickness(44, 23, 0, 67);
            grid.Children.Add(label3);

            System.Windows.Controls.Label label4 = new System.Windows.Controls.Label();
            label4.Content = order.LastChangingStatusTime.ToString();
            label4.Margin = new Thickness(2, 67, 18, 32);
            grid.Children.Add(label4);

            System.Windows.Controls.Label label5 = new System.Windows.Controls.Label();
            label5.Content = order.Status.Name;
            label5.Margin = new Thickness(2, 90, 45, 1);
            grid.Children.Add(label5);

            grid.MouseLeftButtonDown += (sender, e) =>
            {
                OrderDetails orderDetails = new OrderDetails(order);
                orderDetails.ShowDialog();
            };

            return grid;
        }

        private void LoadDiscountTableWithData()
        {
            var discounts = _discountService.GetAll();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (discounts.Count == 0) continue;
                    var grid = CreateDiscountGrid(discounts.First());


                    Grid.SetRow(grid, i);
                    Grid.SetColumn(grid, j);

                    this.DiscountTableGrid.Children.Add(grid);
                    discounts.RemoveAt(0);
                }
            }
        }

        private Grid CreateDiscountGrid(Discount discount)
        {
            Grid grid = new Grid();
            grid.Background = System.Windows.Media.Brushes.Gray;
            grid.Margin = new Thickness(7);

            Grid.SetRow(grid, 0);
            Grid.SetColumn(grid, 0);

            System.Windows.Controls.Label label1 = new System.Windows.Controls.Label();
            label1.Content = discount.ID;
            label1.Margin = new Thickness(0, 0, 74, 29);

            System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
            label2.Content = discount.Type.Type == "Percentage" ? $"{discount.Value}%" : $"{discount.Value}$";
            label2.FontSize = 20;
            label2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label2.Margin = new Thickness(10, 0, 10, 24);

            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            button.Content = "Apply";
            button.Click += (sender, e) =>
            {
                _currentOrder.Discount = discount;
            };
            button.Margin = new Thickness(37, 33, 37, 4);

            grid.Children.Add(label1);
            grid.Children.Add(label2);
            grid.Children.Add(button);

            return grid;
        }


        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            SetCreateOrderPanel();
        }
        private void CreateOrderButton1_Click(object sender, RoutedEventArgs e)
        {
            _currentOrder.Visitor = _currentVisitor;
            var id = _orderService.Insert(_currentOrder);

            foreach (var detail in _currentOrder.OrderDetails)
            {
                _orderDetailService.InsertCategory(id, detail);
            }

            _currentOrder = new Order();
            _currentOrder.OrderDetails = new List<OrderDetail>();
            _currentOrder.Visitor = _currentVisitor;

            SetOrdersPanel();
        }

        private void MenuButton_Click_1(object sender, RoutedEventArgs e)
        {
            SetMenuPanel();
        }

        private void DiscountButton_Click(object sender, RoutedEventArgs e)
        {
            SetDiscountPanel();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            SetOrdersPanel();
        }

        private void PersonButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_role)
            {
                case Role.Visitor:
                    PersonInfo personInfo = new PersonInfo(_currentVisitor.Person, _role);
                    personInfo.ShowDialog();
                    break;
                case Role.Employee:
                    PersonInfo personInfo2 = new PersonInfo(_currentWaiter.Person, _role);
                    personInfo2.ShowDialog();
                    break;
                default:
                    break;
            }

        }

        private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SetCreateOrderPanelVisible(false);
            SetDiscountsPanelVisible(false);
            SetOrdersPanelVisible(false);

            ClearMenuPanel(true);
            SetMenuPanelVisible(true);
            LoadMenuTabControlWithData(true);
        }

        private Dictionary<Role, KeyValuePair<string, string>> accounts = new()
        {
            {Role.Visitor, KeyValuePair.Create("visitor", "1111") },
            {Role.Employee, KeyValuePair.Create("employee", "1111") }
        };

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            LogIn();
            SetMenu();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            _role = Role.Guest;
            SetMenu();
        }

        private void SetStaticticGridVisibility(bool visible)
        {
            this.StatisticGrid.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        private void StatisticButton_Click(object sender, RoutedEventArgs e)
        {
            SetCreateOrderPanelVisible(false);
            SetMenuPanelVisible(false);
            SetDiscountsPanelVisible(false);
            SetOrdersPanelVisible(false);

            SetStaticticGridVisibility(true);
            Dictionary<DateOnly, double> orders = _orderService.GetAll(_orderDetailService, _dishService).GroupBy(x => new DateOnly(x.CreationTime.Year, x.CreationTime.Month, x.CreationTime.Day), x => x.OrderDetails.Sum(y => y.Amount * y.UnitPrice)).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum());
            
            this.PlotView.Model = BuildPlot(orders);
        }

        public PlotModel BuildPlot(Dictionary<DateOnly, double> data)
        {
            var plotModel = new PlotModel { Title = "Date" };

            var dateAxis = new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd.MM.yy" };
            plotModel.Axes.Add(dateAxis);

            var moneyAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Income" };
            plotModel.Axes.Add(moneyAxis);

            var series = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White
            };

            foreach (var entry in data)
            {
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(entry.Key.Year, entry.Key.Month, entry.Key.Day)), entry.Value));
            }

            plotModel.Series.Add(series);

            return plotModel;
        }
    }
}