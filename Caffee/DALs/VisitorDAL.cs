using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//TODO

namespace RestaurantAPI.Dal
{
    public class VisitorDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public VisitorDAL(string connectionstring)
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

        ~VisitorDAL()
        {
            Dispose(true);
        }

        public List<Visitor> GetAll()
        {
            OpenConnection();
            List<Visitor> visitors = new List<Visitor>();

            string sql =
                @"SELECT Visitors.ID, Persons.ID as PersonID, Persons.FirstName, Persons.LastName, Persons.BirthDate FROM Visitors inner join Persons on Persons.ID = Visitors.Person";

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                var dateTime = (DateTime)dataReader["BirthDate"];
                visitors.Add(new Visitor
                {
                    ID = (int)dataReader["ID"],
                    Person = new Person()
                    {
                        ID = (int)dataReader["PersonID"],
                        FirstName = (string)dataReader["FirstName"],
                        LastName = (string)dataReader["LastName"],
                        BirthDate = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day)
                    }
                });
            }

            dataReader.Close();
            return visitors;
        }

        public List<Waiter> GetAllWaiters()
        {
            OpenConnection();
            List<Waiter> visitors = new List<Waiter>();

            string sql =
                @"SELECT Waiters.ID, Persons.ID as PersonID, Persons.FirstName, Persons.LastName, Persons.BirthDate FROM Waiters inner join Persons on Persons.ID = Waiters.Person";

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                var dateTime = (DateTime)dataReader["BirthDate"];
                visitors.Add(new Waiter
                {
                    ID = (int)dataReader["ID"],
                    Person = new Person()
                    {
                        ID = (int)dataReader["PersonID"],
                        FirstName = (string)dataReader["FirstName"],
                        LastName = (string)dataReader["LastName"],
                        BirthDate = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day)
                    }
                });
            }

            dataReader.Close();
            return visitors;
        }
    }
}