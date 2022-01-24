using Microsoft.Extensions.DependencyInjection;

namespace RoleManagement.SharedKernel.Builder
{
    public interface IRoleManagerBuilder
    {
        IServiceCollection Services { get; }
    }
}