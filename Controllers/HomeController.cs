using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using AADSqlAuthRepro.Models;
using Microsoft.AspNetCore.Mvc;

namespace AADSqlAuthRepro.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(DatabaseDetails database)
        {
            if (!string.IsNullOrWhiteSpace(database.Server))
            {
                // Connect to Azure SQL with AAD password auth

                SqlConnectionStringBuilder csb = GetConnectionStringBuilder(database);

                try
                {                    
                    database.SuccessfulConnection = await IsAzureSqlAsync(csb.ConnectionString);
                }
                catch (Exception e)
                {
                    database.ErrorMessage = e.Message;
                    Exception inner = e.InnerException;

                    while (!(inner is null))
                    {
                        database.ErrorMessage = inner.Message;
                        inner = inner.InnerException;
                    };
                }
            }
            
            return View(database);
        }

        private static SqlConnectionStringBuilder GetConnectionStringBuilder(DatabaseDetails database) => new SqlConnectionStringBuilder
        {
            DataSource = database.Port == 0 ? database.Server : $"{database.Server},{database.Port}",
            InitialCatalog = database.DatabaseName,
            UserID = database.UserName,
            Password = database.Password,
            Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword,
            IntegratedSecurity = false,
            Encrypt = true,
            TrustServerCertificate = false,
            ConnectRetryCount = 5,
            ConnectTimeout = 30
        };

        private async Task<bool> IsAzureSqlAsync(string connectionString)
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = "SELECT SERVERPROPERTY('Edition')"
            };

            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            return reader.GetString(0).Equals("SQL Azure", StringComparison.OrdinalIgnoreCase);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
