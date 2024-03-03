using JumpStartCS.Orleans.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.Configure<ClusterOptions > (options =>
                    {
                        options.ClusterId = "JumpstartCSCluster";
                        options.ServiceId = "JumpstartCSService";
                    });

        siloBuilder.AddMemoryGrainStorage(name: "accountStore");
        siloBuilder.AddMemoryGrainStorage(name: "customerStore");

        siloBuilder.UseLocalhostClustering(siloPort: 30000, gatewayPort: 30001);

        //Configure the Analytics Service DI inside a Silo running outside ASP.NET 
        //siloBuilder.ConfigureServices(services =>
        //{
        //    services.AddSingleton<IAnalyticsService, AnalyticsService>();
        //});
    })
    .RunConsoleAsync();