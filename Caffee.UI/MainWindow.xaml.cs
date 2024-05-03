using Caffee.Models;
using Microsoft.Data.SqlClient;
using RestaurantAPI.Dal;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Caffee.UI
{
    public partial class MainWindow : System.Windows.Window
    {
        private Order _currentOrder = new Order();
        private Visitor _currentVisitor = new Visitor();
        private CategoryDAL _categoryService;
        private DishDAL _dishService;
        private DiscountDAL _discountService;
        private OrderDAL _orderService;
        private OrderDetailDAL _orderDetailService;
        private VisitorDAL _visitorService;
        private string _filterQuery = "";

        private void SetMock()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = "MRAKIV",
                InitialCatalog = "Caffee",
                TrustServerCertificate = true,
                IntegratedSecurity = true,
            };

            string connectionString = sqlConnectionStringBuilder.ConnectionString;
            _categoryService = new CategoryDAL(connectionString);
            _dishService = new DishDAL(connectionString);
            _discountService = new DiscountDAL(connectionString);
            _orderService = new OrderDAL(connectionString);
            _orderDetailService = new OrderDetailDAL(connectionString);
            _visitorService = new VisitorDAL(connectionString);

            _currentVisitor = _visitorService.GetAll().First();

            _currentOrder.OrderDetails = new List<OrderDetail>();
        }

        public MainWindow()
        {
            InitializeComponent();
            SetMock();
        }

        //Visibility
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
        //

        //Setting panels
        private void SetMenuPanel()
        {
            SetCreateOrderPanelVisible(false);
            SetDiscountsPanelVisible(false);
            SetOrdersPanelVisible(false);

            ClearMenuPanel();
            SetMenuPanelVisible(true);
            LoadMenuTabControlWithData();
        }
        private void SetCreateOrderPanel()
        {
            SetMenuPanelVisible(false);
            SetDiscountsPanelVisible(false);
            SetOrdersPanelVisible(false);

            ClearCreateOrderPanel();
            SetCreateOrderPanelVisible(true);

            LoadOrderDetailPanelWithData();
        }
        private void SetDiscountPanel()
        {
            SetCreateOrderPanelVisible(false);
            SetMenuPanelVisible(false);
            SetOrdersPanelVisible(false);

            SetDiscountsPanelVisible(true);
        }
        private void SetOrdersPanel()
        {
            SetCreateOrderPanelVisible(false);
            SetMenuPanelVisible(false);
            SetDiscountsPanelVisible(false);

            SetOrdersPanelVisible(true);

            LoadOrdersWithData();
        }
        //
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
                grid.Background = System.Windows.Media.Brushes.Aqua;
                grid.Height = 60;

                var dishLabel = new System.Windows.Controls.Label();
                dishLabel.Content = item.Dish.Name;
                dishLabel.Margin = new Thickness(30, 15, 705, 15);
                dishLabel.Height = 30;
                grid.Children.Add(dishLabel);

                var amountTextBox = new System.Windows.Controls.TextBox();
                amountTextBox.Text = item.Amount.ToString();
                amountTextBox.Margin = new Thickness(333, 15, 632, 15);
                grid.Children.Add(amountTextBox);

                var unitPriceLabel = new System.Windows.Controls.Label();
                unitPriceLabel.Content = item.UnitPrice.ToString();
                unitPriceLabel.Margin = new Thickness(414, 15, 527, 15);
                grid.Children.Add(unitPriceLabel);

                var priceLabel = new System.Windows.Controls.Label();
                priceLabel.Content = (item.UnitPrice * item.Amount).ToString();
                priceLabel.Margin = new Thickness(511, 15, 456, 15);
                grid.Children.Add(priceLabel);

                var deleteButton = new System.Windows.Controls.Button();
                deleteButton.Content = "Delete";
                deleteButton.Click += (sender, e) =>
                {
                    RemoveOrderDetailFromOrder(item);
                };
                deleteButton.Margin = new Thickness(594, 15, 10, 15);
                grid.Children.Add(deleteButton);

                this.OrderDetailsStackPanel.Children.Add(grid);
            }
        }
        
        private void ClearCreateOrderPanel()
        {
            this.OrderDetailsStackPanel.Children.Clear();
        }
        
        private void LoadMenuTabControlWithData()
        {
            var categories = _categoryService.GetAll();
            var dishesDictionary = _dishService.GetAll().Where(x => categories.Any(y => x.Category.ID == y.ID)).ToList().GroupBy(x => x.Category.ID).ToDictionary(x => x.Key, x => x.ToList());
            
            foreach (var category in categories)
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = category.Name;

                ScrollViewer scrollViewer = new ScrollViewer();
                tabItem.Content = scrollViewer;

                Grid outerGrid = new Grid();
                scrollViewer.Content = outerGrid;
                int margin = 0;

                this.MenuTabControl.Items.Add(tabItem);

                foreach (var dish in dishesDictionary[category.ID])
                {
                    Grid grid = new Grid();
                    grid.Margin = new Thickness(26, 0 + margin, 23, 535 - margin);
                    grid.Background = System.Windows.Media.Brushes.Gray;
                    
                    margin += 110;

                    System.Windows.Controls.Label dishLabel = new System.Windows.Controls.Label();
                    dishLabel.Content = dish.Name;
                    dishLabel.Margin = new Thickness(10, 19, 549, 40);
                    dishLabel.Background = System.Windows.Media.Brushes.Pink;
                    dishLabel.Height = 30;

                    System.Windows.Controls.Label priceLabel = new System.Windows.Controls.Label();
                    priceLabel.Content = dish.Price.ToString();
                    priceLabel.Background = System.Windows.Media.Brushes.Green;
                    priceLabel.Margin = new Thickness(10, 49, 549, 10);

                    System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
                    textBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    textBox.Margin = new Thickness(200, 19, 249, 10);
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.Text = dish.Description;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.Width = 662;
                    textBox.Height = 69;

                    //тригер на зміну часу творення замовлення

                    System.Windows.Controls.Button addButton = new System.Windows.Controls.Button();
                    addButton.Click += (sender, e) =>
                    {
                        AddDishToOrder(dish);
                    };
                    addButton.Content = "Add";
                    addButton.Margin = new Thickness(559, 15, 10, 23);

                    grid.Children.Add(dishLabel);
                    grid.Children.Add(priceLabel);
                    grid.Children.Add(textBox);
                    grid.Children.Add(addButton);

                    outerGrid.Children.Add(grid);
                }
            }
        }
        private void AddDishToOrder(Dish dish)
        {
            if(_currentOrder.OrderDetails.Any(x => x.ID == dish.ID))
            {
                var currentDish = _currentOrder.OrderDetails.Where(x => x.ID == dish.ID).First();
                currentDish.Amount++;
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
        private void ClearMenuPanel()
        {
            this.MenuTabControl.Items.Clear();
            this.MinTextBox.Text = string.Empty;
            this.MaxTextBox.Text = string.Empty;
        }

        private void LoadOrdersWithData()
        {
            var orders = _orderService.GetAll(_currentVisitor, _orderDetailService, _dishService);
            // Створення та додавання внутрішніх Grid з даними
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (orders.Count == 0) continue;
                    var grid = GetOrderGrid(orders.First());


                    Grid.SetRow(grid, i);
                    Grid.SetColumn(grid, j);

                    this.OrdersTableGrid.Children.Add(grid);
                    orders.RemoveAt(0);
                }
            }

            var rt = this.OrdersTableGrid.Children[0];
            rt = this.OrdersTableGrid.Children[1];
            rt = this.OrdersTableGrid.Children[2];
            rt = this.OrdersTableGrid.Children[3];
        }
    

        private Grid GetOrderGrid(Order order)
        {
            Grid grid = new Grid();
            grid.Background = System.Windows.Media.Brushes.Bisque;
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
            //var orders = _orderService.GetAll(_currentVisitor.Person);

            //foreach (var order in orders)
            //{
            //    Grid grid = new Grid();
            //    grid.Margin = new Thickness(26, 0 + margin, 23, 535 - margin);
            //    grid.Background = System.Windows.Media.Brushes.Gray;

            //    margin += 110;

            //    System.Windows.Controls.Label dishLabel = new System.Windows.Controls.Label();
            //    dishLabel.Content = dish.Name;
            //    dishLabel.Margin = new Thickness(10, 19, 549, 40);
            //    dishLabel.Background = System.Windows.Media.Brushes.Pink;
            //    dishLabel.Height = 30;

            //    System.Windows.Controls.Label priceLabel = new System.Windows.Controls.Label();
            //    priceLabel.Content = dish.Price.ToString();
            //    priceLabel.Background = System.Windows.Media.Brushes.Green;
            //    priceLabel.Margin = new Thickness(10, 49, 549, 10);

            //    System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
            //    textBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //    textBox.Margin = new Thickness(200, 19, 249, 10);
            //    textBox.TextWrapping = TextWrapping.Wrap;
            //    textBox.Text = dish.Description;
            //    textBox.VerticalAlignment = VerticalAlignment.Center;
            //    textBox.Width = 662;
            //    textBox.Height = 69;

            //    //тригер на зміну часу творення замовлення

            //    System.Windows.Controls.Button addButton = new System.Windows.Controls.Button();
            //    addButton.Click += (sender, e) =>
            //    {
            //        AddDishToOrder(dish);
            //    };
            //    addButton.Content = "Add";
            //    addButton.Margin = new Thickness(559, 15, 10, 23);

            //    grid.Children.Add(dishLabel);
            //    grid.Children.Add(priceLabel);
            //    grid.Children.Add(textBox);
            //    grid.Children.Add(addButton);

            //    this.DiscountTableGrid.Children.Add(grid);

            //}
        }


        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            SetCreateOrderPanel();
        }
        private void CreateOrderButton1_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
    }
}