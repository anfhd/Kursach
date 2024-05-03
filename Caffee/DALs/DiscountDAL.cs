using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//TODO

namespace RestaurantAPI.Dal
{
    public class DiscountDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public DiscountDAL(string connectionstring)
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

        ~DiscountDAL()
        {
            Dispose(true);
        }

        public List<Discount> GetAll()
        {
            OpenConnection();
            List<Discount> discounts = new List<Discount>();

            string sql =
                @"SELECT Discounts.ID, DiscountTypes.ID, DiscountTypes.Type, Discounts.Value FROM Discounts INNER JOIN DiscountTypes ON Discounts.Type = DiscountTypes.ID";

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                discounts.Add(new Discount
                {
                    ID = (int)dataReader["Discounts.ID"],
                    Type = new DiscountType()
                    {
                        ID = (int)dataReader["DiscountTypes.ID"],
                        Type = (string)dataReader["DiscountTypes.Type"]
                    },
                    Value = (double)dataReader["Discounts.Value"]
                });
            }

            dataReader.Close();
            return discounts;
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