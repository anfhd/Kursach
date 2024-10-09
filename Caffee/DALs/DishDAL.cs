using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//TODO

namespace RestaurantAPI.Dal
{
    public class DishDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public DishDAL(string connectionstring)
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

        ~DishDAL()
        {
            Dispose(true);
        }
        public List<Dish> GetAll()
        {
            OpenConnection();
            List<Dish> dishes = new List<Dish>();

            string sql =
                "SELECT D.ID AS DishID, D.Name AS DishName, D.Description, D.Price, C.ID AS CategoryID, C.Name AS CategoryName FROM Dishes AS D INNER JOIN Categories AS C ON D.Category = C.ID;";

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                dishes.Add(new Dish
                {
                    ID = (int)dataReader["DishID"],
                    Name = (string)dataReader["DishName"],
                    Description = dataReader["Description"] is DBNull? "No description :(" : (string)dataReader["Description"],
                    Price = Convert.ToDouble((decimal)dataReader["Price"]),
                    Category = new Category()
                    {
                        ID = (int)dataReader["CategoryID"],
                        Name = (string)dataReader["CategoryName"]
                    }
                });
            }

            dataReader.Close();
            return dishes;
        }
    }
}