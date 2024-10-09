using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//TODO

namespace RestaurantAPI.Dal
{
    public class OrderDetailDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public OrderDetailDAL(string connectionstring)
        => _connectionString = connectionstring;

        private void OpenConnection()
        {
            _sqlConnection = new SqlConnection
            {
                ConnectionString = _connectionString
            };
            _sqlConnection.Open();
        }

        private void CloseConnection()
        {
            if (_sqlConnection?.State != ConnectionState.Closed)
            {
                _sqlConnection?.Close();
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _sqlConnection?.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OrderDetailDAL()
        {
            Dispose(true);
        }

        public List<OrderDetail> GetAll(int orderID, DishDAL dishService)
        {
            OpenConnection();
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            string sql =
                @"SELECT ID, Dish, Amount, UnitPrice FROM OrderDetails where OrderNumber = " + orderID;

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                int dishID = (int)dataReader["Dish"];
                orderDetails.Add(new OrderDetail
                {
                    ID = (int)dataReader["ID"],
                    Dish = dishService.GetAll().Where(x => dishID == x.ID).First(),
                    Amount = (int)dataReader["Amount"],
                    UnitPrice = Convert.ToDouble((decimal)dataReader["UnitPrice"])
                });
            }

            dataReader.Close();
            return orderDetails;
        }

        public void InsertCategory(int orderId, OrderDetail orderDetail)
        {
            OpenConnection();

            string sql = $"INSERT INTO OrderDetails (OrderNumber, Dish, Amount, UnitPrice) Values ({orderId}, {orderDetail.Dish.ID}, {orderDetail.Amount}, {orderDetail.UnitPrice.ToString().Replace(',', '.')})";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}