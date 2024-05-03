using Caffee.Models;
using Microsoft.Data.SqlClient;
using System.Data;

//TODO

namespace RestaurantAPI.Dal
{
    public class OrderDAL : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection = null;
        bool _disposed = false;

        public OrderDAL(string connectionstring)
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

        ~OrderDAL()
        {
            Dispose(true);
        }

        public List<Order> GetAll(Visitor visitor, OrderDetailDAL orderDetailService, DishDAL dishService)
        {
            OpenConnection();
            List<Order> orders = new List<Order>();

            string sql =
                @"select Orders.ID, Tables.ID AS TableID, Tables.SeatsNumber, Statuses.ID as StatusID, Statuses.Name as StatusName, CreationTime, LastChangingStatusTime, Waiters.ID as WaiterID, Persons.ID as WaiterPersonID, Persons.FirstName as WaiterFirstName, Persons.LastName as WaiterLastName, Persons.BirthDate as WaiterBirthDate, Discounts.ID as DiscountID, Discounts.Value, DiscountTypes.ID as DiscountTypeID, DiscountTypes.Type from Orders
inner join Tables on Tables.ID = Orders.TableNumber
inner join Statuses on Statuses.ID = Orders.Status
inner join Waiters on Waiters.ID = Orders.Waiter
inner join Persons on Waiters.Person = Persons.ID
left join Discounts on Discounts.ID = Orders.Discount
left join DiscountTypes on Discounts.Type = DiscountTypes.ID
where Orders.Visitor = " + visitor.ID;

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.CommandType = CommandType.Text;

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                var dateTime = (DateTime)dataReader["WaiterBirthDate"];
                orders.Add(new Order
                {
                    ID = (int)dataReader["ID"],
                    Table = new Table()
                    {
                        ID = (int)dataReader["TableID"],
                        SeatsNumber = (int)dataReader["SeatsNumber"]
                    },
                    Status = new Status()
                    {
                        ID = (int)dataReader["StatusID"],
                        Name = (string)dataReader["StatusName"]
                    },
                    CreationTime = (DateTime)dataReader["CreationTime"],
                    LastChangingStatusTime = (DateTime)dataReader["LastChangingStatusTime"],
                    Waiter = new Waiter()
                    {
                        ID = (int)dataReader["WaiterID"],
                        Person = new Person()
                        {
                            ID = (int)dataReader["WaiterPersonID"],
                            FirstName = (string)dataReader["WaiterFirstName"],
                            LastName = (string)dataReader["WaiterLastName"],
                            BirthDate = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day)
                        }
                    },
                    Visitor = visitor,
                    Discount = dataReader["DiscountID"] is DBNull? null : new Discount()
                    {
                        ID = (int)dataReader["DiscountID"],
                        Type = new DiscountType()
                        {
                            ID = (int)dataReader["DiscountTypeID"],
                            Type = (string)dataReader["Type"]
                        },
                        Value = (double)dataReader["Value"]
                    },
                    OrderDetails = orderDetailService.GetAll((int)dataReader["ID"], dishService)
                });
            }

            dataReader.Close();
            return orders;
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