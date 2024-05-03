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

        public Category? GetCategory(int id)
        {
            OpenConnection();
            Category? category = null;

            string sql = $"SELECT ID, Name FROM Categories WHERE ID = {id}";
            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                category = new Category
                {
                    ID = (int)dataReader["ID"],
                    Name = (string)dataReader["Name"],
                };
            }

            dataReader.Close();
            return category;
        }

        public void InsertCategory(string name)
        {
            OpenConnection();
            string sql = $"INSERT INTO Categories (Name) VALUES ('{name}')";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        public void InsertCategory(Category category)
        {
            OpenConnection();

            string sql = "INSERT INTO Categories (Name) Values ('{car.Name}')";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        public void DeleteCategory(int id)
        {
            OpenConnection();

            string sql = $"DELETE FROM Categories WHERE ID ={id}";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                try
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    Exception error = new NotImplementedException();
                    throw error;
                }
            }

            CloseConnection();
        }

        public void UpdateCategory(int id, string newName)
        {
            OpenConnection();

            string sql = $"UPDATE Categories SET Name = '{newName}' WHERE ID = '{id}'";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}