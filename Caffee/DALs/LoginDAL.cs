using Caffee.Models;
using Microsoft.Data.SqlClient;
using RestaurantAPI.Dal;
using System.Data;

namespace Caffee.DALs
{
    public class LoginDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public LoginDAL(string connectionstring)
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

        ~LoginDAL()
        {
            Dispose(true);
        }

        public void TryConnect()
        {
            OpenConnection();
            CloseConnection();
        }
    }
}
