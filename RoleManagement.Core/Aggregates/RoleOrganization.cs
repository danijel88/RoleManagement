using System;
using System.Collections.Generic;
using System.Linq;
using GuardClauses;
using RoleManagement.SharedKernel;

namespace RoleManagement.Core.Aggregates
{
    public class RoleOrganization
    {
        private readonly List<RoleOrganizationRoles> _roleOrganizationRoles = new List<RoleOrganizationRoles>();
        public RoleOrganization(string name)
        {
            GuardClause.ArgumentIsNotNull(name, nameof(name));
            GuardClause.MaximumLength(name, nameof(name), RoleManagementConstants.ROLE_ORGANIZATION_DEFAULT_NAME_LENGTH);
            Name = name;
        }

        public int Id { get; protected set; }
        public string Name { get; private set; }
        public IEnumerable<RoleOrganizationRoles> RoleOrganizationRoles => _roleOrganizationRoles;

        public void AddRoleOrganizationRoles(RoleOrganizationRoles roleOrganizationRoles)
        {
            if (_roleOrganizationRoles.Any(x => x.RoleId == roleOrganizationRoles.RoleId))
                throw new ArgumentException("Role already added");
            _roleOrganizationRoles.Add(roleOrganizationRoles);
        }

        public void RemoveRoleOrganizationRoles(int roleOrganizationRoleId)
        {
            GuardClause.IsZeroOrNegative(roleOrganizationRoleId, nameof(roleOrganizationRoleId));
            var roleOrganizationRoles = _roleOrganizationRoles.FirstOrDefault(w => w.Id == roleOrganizationRoleId);
            GuardClause.ArgumentIsNotNull(roleOrganizationRoles, nameof(roleOrganizationRoles));
            _roleOrganizationRoles.Remove(roleOrganizationRoles);
        }
        
    }
}