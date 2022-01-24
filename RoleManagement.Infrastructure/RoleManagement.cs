using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RoleManagement.Core.Aggregates;
using RoleManagement.Core.Exceptions;
using RoleManagement.Core.Interfaces;

namespace RoleManagement.Infrastructure
{
    public class RoleManagement : IRoleManagement
    {
        private readonly SqlOptions _options;
        private readonly ILogger<RoleManagement> _logger;

        public RoleManagement(SqlOptions options, ILogger<RoleManagement> logger)
        {
            _options = options;
            _logger = logger;
        }
        public bool AddRoleOrganization(RoleOrganization roleOrganization)
        {
            try
            {
                if (AlreadyExist(roleOrganization.Name))
                    throw new AlreadyExistException($"RoleOrganization {roleOrganization.Name} already exist.");
                using var sqlConnection = new SqlConnection(_options.ConnectionString);
                var query = "INSERT INTO RoleOrganizations VALUES(@organizationName)";
                using var sqlCommand = new SqlCommand(query,sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@organizationName", roleOrganization.Name);
                int changes = sqlCommand.ExecuteNonQuery();
                return changes > 0 ? true:false;
            }
            catch (Exception e)
            {
                _logger.LogError(e,"An error occurred while inserting in db");
                return false;
            }
        }

        public bool DeleteRoleOrganization(int id)
        {
            try
            {
                using var sqlConnection = new SqlConnection(_options.ConnectionString);
                var query = "DELETE FROM RoleOrganizations WHERE Id = @id";
                using var sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@id", id);
                int changes = sqlCommand.ExecuteNonQuery();
                return changes > 0 ? true : false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating the db.");
                return false;
            }
        }

        public bool AddRoleOrganizationRoles(RoleOrganization roleOrganization)
        {
            int changes = 0;
            try
            {
                foreach (RoleOrganizationRoles roleOrganizationRole in roleOrganization.RoleOrganizationRoles)
                {
                    if (!RoleAlreadyInsideRoleOrganization(roleOrganizationRole.RoleId))
                    {
                        using var sqlConnection = new SqlConnection(_options.ConnectionString);
                        var query = "INSERT INTO RoleOrganizationRoles VALUES(@roleOrganizationId,@roleId)";
                        using var sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@roleOrganizationId", roleOrganizationRole.RoleOrganizationId);
                        sqlCommand.Parameters.AddWithValue("@roleId", roleOrganizationRole.RoleId);
                        changes += sqlCommand.ExecuteNonQuery();
                    }
                }

                return changes > 0 ? true : false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating the db.");
                return false;
            }
        }

        public async Task<bool> RemoveRoleOrganizationRoles(RoleOrganization roleOrganization)
        {
            int changes = 0;
            var allRolesByRoleOrganization = await GetAllRolesByRoleOrganizationId(roleOrganization.Id);
            foreach (RoleOrganizationRoles roleOrganizationRoles in allRolesByRoleOrganization)
            {
                if (roleOrganization.RoleOrganizationRoles.All(x => x.Id != roleOrganizationRoles.Id))
                {
                    using var sqlConnection = new SqlConnection(_options.ConnectionString);
                    var query = "DELETE FROM RoleOrganizationRoles WHERE Id = @id";
                    using var sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@id", roleOrganizationRoles.Id);
                    changes += await sqlCommand.ExecuteNonQueryAsync();
                }
            }

            return changes > 0 ? true : false;
        }

        public async Task<IList<RoleOrganizationRoles>> GetAllRolesByRoleOrganizationId(int roleOrganizationId)
        {
            using var sqlConnection = new SqlConnection(_options.ConnectionString);
            var query = "SELECT * FROM RoleOrganizationRoles WHERE RoleOrganizationId = @roleOrganizationId";
            using var sqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlCommand.Parameters.AddWithValue("@roleOrganizationId", roleOrganizationId);
            var reader = await sqlCommand.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                var response = new List<RoleOrganizationRoles>();
                while (reader.Read())
                {
                    response.Add(new RoleOrganizationRoles(Convert.ToInt32(reader[1]),reader[2].ToString()));
                }

                return response;
            }
            else
            {
                return new List<RoleOrganizationRoles>();
            }
        }

        private bool AlreadyExist(string name)
        {
            using var sqlConnection = new SqlConnection(_options.ConnectionString);
            var query = "SELECT * FROM RoleOrganizations where Name = @name";
            using var sqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlCommand.Parameters.AddWithValue("@name", name);
            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
                return true;
            return false;
        }

        private bool RoleAlreadyInsideRoleOrganization(string roleId)
        {
            using var sqlConnection = new SqlConnection(_options.ConnectionString);
            var query = "SELECT * FROM RoleOrganizationRoles WHERE RoleId =@roleId";
            using var sqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            sqlCommand.Parameters.AddWithValue("@roleId", roleId);
            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
                return true;
            return false;
        }
        
    }
}