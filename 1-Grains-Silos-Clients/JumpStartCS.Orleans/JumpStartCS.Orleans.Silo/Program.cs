using JumpStartCS.Orleans.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering(siloPort: 30000, gatewayPort: 30001);

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

        siloBuilder.AddMemoryGrainStorage(name: "memoryStorage");

        siloBuilder.AddAzureTableGrainStorage(
            name: "tableStorage",
            configureOptions: options =>
            {
                options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
            });

        siloBuilder.UseAzureTableGrainDirectoryAsDefault(configureOptions: options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

        siloBuilder.UseInMemoryReminderService();

        //siloBuilder.ConfigureLogging(builder => {

        //    builder.SetMinimumLevel(LogLevel.Information);    
        //});

        //Configure the Compliance Service DI inside a Silo running outside ASP.NET 
        siloBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<IComplianceService, ComplianceService>();
        });
    })
    .RunConsoleAsync();