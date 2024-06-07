using IdentitySample.Models.Context;
using IdentitySample.Services;
using Kaktos.UserImmediateActions.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitySample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var mainStore = scope.ServiceProvider.GetRequiredService<IImmediateActionsStore>();

                var syncImmediateActionsStores = new SyncImmediateActionsStores(dbContext, mainStore);
                await syncImmediateActionsStores.SyncImmediateActionsFromPermanentStoreToMainStoreAsync();
            }

            await host.RunAsync();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
