using GuardClauses;

namespace RoleManagement.Core.Aggregates
{
    public class RoleOrganizationRoles
    {
        public RoleOrganizationRoles(int roleOrganizationId,string roleId)
        {
            GuardClause.IsZeroOrNegative(roleOrganizationId,nameof(roleOrganizationId));
            RoleOrganizationId = roleOrganizationId;
            RoleId = roleId;
        }

        public int Id { get; protected set; }
        public int RoleOrganizationId { get; private set; }
        public string RoleId { get; private set; }
        
    }
}