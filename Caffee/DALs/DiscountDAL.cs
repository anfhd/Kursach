using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//
//

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
                @"SELECT Discounts.ID, DiscountTypes.ID as DiscountTypesID, DiscountTypes.Type, Discounts.Value FROM Discounts INNER JOIN DiscountTypes ON Discounts.Type = DiscountTypes.ID";

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
                    ID = (int)dataReader["ID"],
                    Type = new DiscountType()
                    {
                        ID = (int)dataReader["DiscountTypesID"],
                        Type = (string)dataReader["Type"]
                    },
                    Value = Convert.ToDouble((decimal)dataReader["Value"])
                });
            }

            dataReader.Close();
            return discounts;
        }
    }
}