using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//TODO

namespace RestaurantAPI.Dal
{
    public class CategoryDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public CategoryDAL(string connectionstring)
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

        ~CategoryDAL()
        {
            Dispose(true);
        }

        public List<Category> GetAll()
        {
            OpenConnection();
            List<Category> categories = new List<Category>();

            string sql =
                @"SELECT ID, Name FROM Categories";

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                categories.Add(new Category
                {
                    ID = (int)dataReader["ID"],
                    Name = (string)dataReader["Name"]
                });
            }

            dataReader.Close();
            return categories;
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

            string sql = $"DELETE FROM Categories WHERE ID ={ id }";
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