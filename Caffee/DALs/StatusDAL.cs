using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//TODO

namespace RestaurantAPI.Dal
{
    public class StatusDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public StatusDAL(string connectionstring)
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

        ~StatusDAL()
        {
            Dispose(true);
        }

        public List<Status> GetAll()
        {
            OpenConnection();
            List<Status> statuses = new List<Status>();

            string sql =
                @"SELECT ID, Name FROM Statuses";

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                statuses.Add(new Status
                {
                    ID = (int)dataReader["ID"],
                    Name = (string)dataReader["Name"]
                });
            }

            dataReader.Close();
            return statuses;
        }
    }
}