using Caffee.Models;
using Microsoft.Data.SqlClient;
using RestaurantAPI.Dal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Caffee.UI
{
    public partial class MainWindow : System.Windows.Window
    {
        private Order _currentOrder = new Order();
        private Visitor _currentVisitor = new Visitor();
        private CategoryDAL _categoryService;
        private DishDAL _dishService;

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

            _currentVisitor.ID = 1;
            _currentVisitor.Person = new Person()
            {
                FirstName = "Olenia",
                LastName = "Markiv",
                BirthDate = new DateOnly(2005, 1, 18)
            };

            _currentOrder.OrderDetails = new List<OrderDetail>()
            {
                new OrderDetail()
                {
                    ID = 1,
                    Amount = 3,
                    UnitPrice = 32.9,
                    Dish = new Dish()
                    {
                        ID = 1,
                        Name = "Cool dish",
                        Category = new Category()
                        {
                            ID = 142,
                            Name = "CoolCategory"
                        },
                        Description = "Cool dish reali",
                        Price = 32.9
                    }
                },
                new OrderDetail()
                {
                    ID = 2,
                    Amount = 13,
                    UnitPrice = 2.9,
                    Dish = new Dish()
                    {
                        ID = 4,
                        Name = "Not So Cool dish",
                        Category = new Category()
                        {
                            ID = 141,
                            Name = "NotCoolCategory"
                        },
                        Description = "Poor dish reali",
                        Price = 2.9
                    }
                },
                new OrderDetail()
                {
                    ID = 2,
                    Amount = 13,
                    UnitPrice = 2.9,
                    Dish = new Dish()
                    {
                        ID = 4,
                        Name = "Not So Cool dish",
                        Category = new Category()
                        {
                            ID = 141,
                            Name = "NotCoolCategory"
                        },
                        Description = "Poor dish reali",
                        Price = 2.9
                    }
                },
                new OrderDetail()
                {
                    ID = 2,
                    Amount = 13,
                    UnitPrice = 2.9,
                    Dish = new Dish()
                    {
                        ID = 4,
                        Name = "Not So Cool dish",
                        Category = new Category()
                        {
                            ID = 141,
                            Name = "NotCoolCategory"
                        },
                        Description = "Poor dish reali",
                        Price = 2.9
                    }
                },
                new OrderDetail()
                {
                    ID = 2,
                    Amount = 13,
                    UnitPrice = 2.9,
                    Dish = new Dish()
                    {
                        ID = 4,
                        Name = "Not So Cool dish",
                        Category = new Category()
                        {
                            ID = 141,
                            Name = "NotCoolCategory"
                        },
                        Description = "Poor dish reali",
                        Price = 2.9
                    }
                },
                new OrderDetail()
                {
                    ID = 2,
                    Amount = 13,
                    UnitPrice = 2.9,
                    Dish = new Dish()
                    {
                        ID = 4,
                        Name = "Not So Cool dish",
                        Category = new Category()
                        {
                            ID = 141,
                            Name = "NotCoolCategory"
                        },
                        Description = "Poor dish reali",
                        Price = 2.9
                    }
                },
                new OrderDetail()
                {
                    ID = 2,
                    Amount = 13,
                    UnitPrice = 2.9,
                    Dish = new Dish()
                    {
                        ID = 4,
                        Name = "Not So Cool dish",
                        Category = new Category()
                        {
                            ID = 141,
                            Name = "NotCoolCategory"
                        },
                        Description = "Poor dish reali",
                        Price = 2.9
                    }
                },
                new OrderDetail()
                {
                    ID = 2,
                    Amount = 13,
                    UnitPrice = 2.9,
                    Dish = new Dish()
                    {
                        ID = 4,
                        Name = "Not So Cool dish",
                        Category = new Category()
                        {
                            ID = 141,
                            Name = "NotCoolCategory"
                        },
                        Description = "Poor dish reali",
                        Price = 2.9
                    }
                }
            };
        }
        public MainWindow()
        {
            InitializeComponent();
            SetMock();
        }

        private void SetCreateOrderPanel()
        {
            ClearCreateOrderPanel();
            SetCreateOrderPanelVisible(true);

            LoadOrderDetailPanelWithData();
        }

        private void SetMenuPanel()
        {
            LoadMenuTabControlWithData();
        }

        private void LoadMenuTabControlWithData()
        {
            var categories = _categoryService.GetAll();
            var dishesDictionary = _dishService.GetAll().GroupBy(x => x.Category.ID).ToDictionary(x => x.Key, x => x.ToList());
            
            foreach (var category in categories)
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = category.Name;

                Grid outerGrid = new Grid();
                tabItem.Content = outerGrid;

                foreach(var dish in dishesDictionary[category.ID])
                {
                    Grid grid = new Grid();
                    grid.Margin = new Thickness(26, 0, 23, 535);

                    System.Windows.Controls.Label dishLabel = new System.Windows.Controls.Label();
                    dishLabel.Content = "Dish";
                    dishLabel.Margin = new Thickness(10, 19, 699, 40);
                    dishLabel.Height = 30;

                    System.Windows.Controls.Label priceLabel = new System.Windows.Controls.Label();
                    priceLabel.Content = "Price";
                    priceLabel.Margin = new Thickness(10, 53, 905, 10);

                    System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
                    textBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    textBox.Margin = new Thickness(191, 0, 0, 0);
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.Text = "TextBox";
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.Width = 662;
                    textBox.Height = 69;

                    System.Windows.Controls.Button addButton = new System.Windows.Controls.Button();
                    addButton.Content = "Add";
                    addButton.Margin = new Thickness(912, 15, 10, 23);

                    // Додавання елементів до grid
                    grid.Children.Add(dishLabel);
                    grid.Children.Add(priceLabel);
                    grid.Children.Add(textBox);
                    grid.Children.Add(addButton);

                    // Створення нового TabItem і додавання grid до нього
                    outerGrid.Children.Add(tabItem);
                }

                this.MenuTabControl.Items.Add(tabItem);
            }
        }

        public void LoadOrderDetailPanelWithData()
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
                deleteButton.Margin = new Thickness(594, 15, 10, 15);
                grid.Children.Add(deleteButton);

                this.OrderDetailsStackPanel.Children.Add(grid);
            }
        }
        private void SetCreateOrderPanelVisible(bool visible)
        {
            this.OrderDetailScrollViewer.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            this.OrderDetailsStackPanel.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            this.CreateOrderButton1.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
        private void ClearCreateOrderPanel()
        {
            this.OrderDetailsStackPanel.Children.Clear();
        }
        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            SetCreateOrderPanel();
        }
        private void CreateOrderButton1_Click(object sender, RoutedEventArgs e)
        {
            SetCreateOrderPanel();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            SetMenuPanel();
        }
    }
}