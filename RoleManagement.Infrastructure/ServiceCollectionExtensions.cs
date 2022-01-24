using Microsoft.Extensions.DependencyInjection;
using RoleManagement.Core.Interfaces;
using RoleManagement.SharedKernel.Builder;

namespace RoleManagement.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IRoleManagerBuilder AddRoleManagement(this IServiceCollection services)
        {
            services.AddScoped<IRoleManagement, RoleManagement>();

            IRoleManagerBuilder builder = new RoleManagerBuilder(services);
            return builder;
        }
    }
}