using JumpStartCS.Orleans.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.Configure<ClusterOptions> (options =>
                    {

                        options.ClusterId = "JumpstartCSCluster";
                        options.ServiceId = "JumpstartCSService";
                    });

        siloBuilder.Configure<GrainCollectionOptions>(options =>
        {
            //How often we want to collect and deactivate grains 
            options.CollectionQuantum = TimeSpan.FromSeconds(20);

            options.CollectionAge = TimeSpan.FromSeconds(30);
        });

        siloBuilder.AddMemoryGrainStorage(name: "accountStore");
        siloBuilder.AddMemoryGrainStorage(name: "customerStore");

        siloBuilder.UseInMemoryReminderService();

        //siloBuilder.ConfigureLogging(builder => {

        //    builder.SetMinimumLevel(LogLevel.Information);    
        //});

        siloBuilder.UseLocalhostClustering(siloPort: 30000, gatewayPort: 30001);

        //Configure the Analytics Service DI inside a Silo running outside ASP.NET 
        siloBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
        });
    })
    .RunConsoleAsync();