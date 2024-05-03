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

        public List<Dish> GetMockAll()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = "MRAKIV",
                InitialCatalog = "Caffee",
                TrustServerCertificate = true,
                IntegratedSecurity = true,
            };

            string connectionString = sqlConnectionStringBuilder.ConnectionString;
            var _categoryService = new CategoryDAL(connectionString);
            var categories = _categoryService.GetAll();

            List<Dish> dishes = new List<Dish>()
            {
                new Dish()
                {
                    ID = 1,
                    Name = "dish1",
                    Category = categories[new Random().Next() % categories.Count],
                    Description = "hfgkhsjf fkjakwe fkwjfb mfkv qvqjwbkv q vkqjv",
                    Price = 13.3
                }
            };

            for(int i=0; i<30; i++)
            {
                dishes.Add(new Dish()
                {
                    ID = i+2,
                    Name = "dish" + (i+2).ToString(),
                    Category = categories[new Random().Next() % categories.Count],
                    Description = "hfgkhsjf fkjakwe fkwjfb mfkv qvqjwbkv q vkqjv",
                    Price = new Random().NextDouble()
                });
            }

            return dishes;
        }
        public List<Dish> GetAll()
        {
            return GetMockAll();

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
                    Description = (string)dataReader["Description"],
                    Price = (double)dataReader["Price"],
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

        public Category? GetCategory(int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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