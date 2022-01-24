using System.Collections.Generic;
using System.Threading.Tasks;
using RoleManagement.Core.Aggregates;

namespace RoleManagement.Core.Interfaces
{
    public interface IRoleManagement
    {
        bool AddRoleOrganization(RoleOrganization roleOrganization);
        bool DeleteRoleOrganization(int id);
        bool AddRoleOrganizationRoles(RoleOrganization roleOrganization);
        Task<bool> RemoveRoleOrganizationRoles(RoleOrganization roleOrganization);
        Task<IList<RoleOrganizationRoles>> GetAllRolesByRoleOrganizationId(int roleOrganizationId);

    }
}