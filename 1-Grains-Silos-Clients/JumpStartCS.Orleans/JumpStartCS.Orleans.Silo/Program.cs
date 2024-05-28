using System.Text.Json;
using JumpStartCS.Orleans.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Serialization;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseAzureStorageClustering(configureOptions: options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

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

        siloBuilder.AddAzureTableGrainStorage(
            name: "globallyDistributedStorage",
            configureOptions: options =>
            {
                options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
            });

        siloBuilder.AddAzureTableGrainStorage(
            name: "locallyDistributedStorage",
            configureOptions: options =>
            {
                options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
            });

        siloBuilder.UseAzureTableGrainDirectoryAsDefault(configureOptions: options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

        siloBuilder.UseAzureTableReminderService(configureOptions: options =>
        {
            options.Configure(o => o.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;"));
        });

        //siloBuilder.ConfigureLogging(builder => {

        //    builder.SetMinimumLevel(LogLevel.Information);    
        //});

        //Option to add json serialization for all type this is just really for inter grain comms
        siloBuilder.Services.AddSerializer(builder =>
        {
            builder.AddJsonSerializer(isSupported: type => true);
        });

        //Configure the Compliance Service DI inside a Silo running outside ASP.NET 
        siloBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<IComplianceService, ComplianceService>();
        });
    })
    .RunConsoleAsync();