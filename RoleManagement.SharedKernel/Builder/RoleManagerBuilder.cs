using Microsoft.Extensions.DependencyInjection;

namespace RoleManagement.SharedKernel.Builder
{
    public class RoleManagerBuilder : IRoleManagerBuilder
    {
        public RoleManagerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}