using Caffee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace Caffee.UI
{
    /// <summary>
    /// Interaction logic for OrderDetails.xaml
    /// </summary>
    public partial class OrderDetails : System.Windows.Window
    {
        private Order _order;
        public OrderDetails()
        {
            InitializeComponent();
        }
        public OrderDetails(Order order) : this()
        {
            _order = order;
            this.IDLabel.Content = order.ID;
            this.TableNumberLabel.Content = order.Table.ID;
            this.SeatsNumberLabel.Content = order.Table.SeatsNumber;
            this.StatusLabel.Content = order.Status.Name;
            this.CreationTimeLabel.Content = order.CreationTime.ToString();
            this.WaiterLabel.Content = $"{order.Waiter.Person.FirstName} {order.Waiter.Person.LastName}";
            this.DiscountLabel.Content = order.Discount is null ? "None :(" : order.Discount.Type.Type == "Percentage" ? $"{order.Discount.Value}%" : $"{order.Discount.Value}$";
            double sum = Math.Round(order.OrderDetails.Select(x => x.UnitPrice * x.Amount).Sum(), 2);

            if (order.Discount is not null)
            {
                if (order.Discount?.Type.Type == "Percentage")
                    sum = (sum / 100.0) * (100.0 - order.Discount.Value);
                else
                {
                    sum = sum - order.Discount.Value;
                }
            }

            this.OrderDetailsStackPanel.Children.Clear();

            foreach(var orderDetail in order.OrderDetails)
            {
                this.OrderDetailsStackPanel.Children.Add(CreateOrderDetail(orderDetail));
            }

            this.SumLabel.Content = $"{sum}$";
        }

        private Grid CreateOrderDetail(OrderDetail orderDetail)
        {
            Grid grid = new();

            Label label1 = new Label();
            label1.Content = orderDetail.Dish.Name;
            label1.Margin = new Thickness(0, 0, 157, 0);

            Label label2 = new Label();
            label2.Content = orderDetail.UnitPrice;
            label2.Margin = new Thickness(136, 0, 121, 0);

            Label label3 = new Label();
            label3.Content = orderDetail.Amount;
            label3.Margin = new Thickness(197, 0, 69, 0);

            Label label4 = new Label();
            label4.Content = Math.Round(orderDetail.Amount * orderDetail.UnitPrice, 2);
            label4.Margin = new Thickness(244, 0, 0, 0);

            // Додавання Label до контейнера, наприклад, до StackPanel
            grid.Children.Add(label1);
            grid.Children.Add(label2);
            grid.Children.Add(label3);
            grid.Children.Add(label4);

            return grid;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
