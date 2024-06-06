using IdentitySample.Models.Context;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Linq;

namespace IdentitySample.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public class ImmediateActionsJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public ImmediateActionsJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await dbContext.ImmediateActions.AnyAsync())
            {
                var expiredActions = await dbContext.ImmediateActions
                    .Where(i => i.ExpirationTime < DateTime.Now)
                    .ToListAsync();

                dbContext.RemoveRange(expiredActions);

                await dbContext.SaveChangesAsync();
            }
        }
    }

}
