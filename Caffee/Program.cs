using Microsoft.Data.SqlClient;
using RestaurantAPI.Dal;

SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
{
    DataSource = "MRAKIV",
    InitialCatalog = "Caffee",
    TrustServerCertificate = true,
    IntegratedSecurity = true,
};

string connectionString = sqlConnectionStringBuilder.ConnectionString;
CategoryDAL categoryDAL = new CategoryDAL(connectionString);

categoryDAL.InsertCategory("tt");
foreach (var category in categoryDAL.GetAll())
{
    Console.WriteLine($"{category.ID} {category.Name}");
}