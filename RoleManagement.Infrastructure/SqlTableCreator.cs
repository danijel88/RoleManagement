using System;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Logging;
using RoleManagement.Core.Aggregates;
using RoleManagement.SharedKernel;

namespace RoleManagement.Infrastructure
{
    public class SqlTableCreator
    {
        private readonly ILogger<SqlTableCreator> _logger;
        private readonly SqlOptions _options;

        public SqlTableCreator(SqlOptions options, ILogger<SqlTableCreator> logger)
        {
            _logger = logger;
            _options = options;
        }


        public void CreateTableQuery()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_options.ConnectionString);
                var sql = GetRoleOrganizationDdl();
                using var cmd = new SqlCommand(sql,connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                sql = GetRoleOrganizationRolesDdl();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while creating table");
            }

        }

        private string GetRoleOrganizationDdl()
        {
            var sql = new StringBuilder();
            sql.AppendLine("IF NOT EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleOrganizations]') AND type IN ('U')) ");
            sql.AppendLine("BEGIN ");
            sql.AppendLine("CREATE TABLE [dbo].[RoleOrganizations] ( ");
            sql.AppendLine("[Id] int IDENTITY(1,1)  NOT NULL, ");
            sql.AppendLine($"[Name] nvarchar({RoleManagementConstants.ROLE_ORGANIZATION_DEFAULT_NAME_LENGTH}) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL );");
            sql.AppendLine("ALTER TABLE [dbo].[RoleOrganizations] SET (LOCK_ESCALATION = TABLE);");
            sql.AppendLine("DBCC CHECKIDENT ('[dbo].[RoleOrganizations]', RESEED, 1);");
            sql.AppendLine("ALTER TABLE [dbo].[RoleOrganizations] ADD CONSTRAINT [PK_RoleOrganizations] PRIMARY KEY CLUSTERED ([Id]) ");
            sql.AppendLine("WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ");
            sql.AppendLine("ON [PRIMARY];");
            sql.AppendLine("END");

            return sql.ToString();
        }
        private string GetRoleOrganizationRolesDdl()
        {
            var sql = new StringBuilder();

            sql.AppendLine("IF NOT EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleOrganizationRoles]') AND type IN ('U')) ");
            sql.AppendLine("BEGIN ");
            sql.AppendLine("CREATE TABLE [dbo].[RoleOrganizationRoles] ( ");
            sql.AppendLine("[Id] int IDENTITY(1,1)  NOT NULL, ");
            sql.AppendLine($"[RoleOrganizationId] int NOT NULL,");
            sql.AppendLine($"[RoleId] nvarchar({RoleManagementConstants.ROLE_ORGANIZATION_DEFAULT_ROLE_ID_LENGTH}) NOT NULL);");
            sql.AppendLine("ALTER TABLE [dbo].[RoleOrganizationRoles] SET (LOCK_ESCALATION = TABLE);");
            sql.AppendLine("DBCC CHECKIDENT ('[dbo].[RoleOrganizationRoles]', RESEED, 1);");
            sql.AppendLine("ALTER TABLE [dbo].[RoleOrganizationRoles] ADD CONSTRAINT [PK_RoleOrganizationRoles] PRIMARY KEY CLUSTERED ([Id]) ");
            sql.AppendLine("WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ");
            sql.AppendLine("ON [PRIMARY];");
            sql.AppendLine("ALTER TABLE [dbo].[RoleOrganizationRoles] ADD CONSTRAINT [FK_RoleOrganizationRoles_RoleOrganization_Id] ");
            sql.AppendLine("FOREIGN KEY ([RoleOrganizationId]) REFERENCES [dbo].[RoleOrganizations] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;");
            sql.AppendLine("ALTER TABLE [dbo].[RoleOrganizationRoles] ADD CONSTRAINT [FK_RoleOrganizationRoles_AspNetRoles_RoleId] ");
            sql.AppendLine("FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;");
            sql.AppendLine("END");

            return sql.ToString();
        }

    }
}