using System.IO;
using Microsoft.Extensions.Configuration;

namespace RoleManagement.SharedKernel
{
    public class ConfigurationBuilderExtension
    {
        public static string GetConnectionString(string connectionName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration.GetConnectionString(connectionName);
        }
    }
}