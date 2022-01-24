using System;
using RoleManagement.SharedKernel.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoleManagement.Core.Interfaces;

namespace RoleManagement.Infrastructure
{
    public static class RoleManagementExtension
    {
        public static IRoleManagerBuilder AddSqlTables(this IRoleManagerBuilder builder, Action<SqlOptions> options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var sqlOptions = new SqlOptions();
            options.Invoke(sqlOptions);

            var sp = builder.Services.BuildServiceProvider();
            var scope = sp.CreateScope();

            var logger = scope.ServiceProvider.GetService<ILogger<SqlTableCreator>>();

            var tablesCreator = new SqlTableCreator(sqlOptions, logger);
            tablesCreator.CreateTableQuery();
            scope.Dispose();
            sp.Dispose();

            builder.Services.AddSingleton(sqlOptions);
            builder.Services.AddScoped<IRoleManagement, RoleManagement>();

            return builder;
        }
    }
}