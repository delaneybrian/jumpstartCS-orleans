using Azure.Storage.Queues;
using JumpStartCS.Orleans.Infrastructure;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.Streams.AzureQueue;
using System.Net.Http;

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
            options.CollectionQuantum = TimeSpan.FromSeconds(20);

            options.CollectionAge = TimeSpan.FromSeconds(30);
        });

        siloBuilder.AddAzureTableTransactionalStateStorageAsDefault(configureOptions: options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

        siloBuilder.AddAzureTableGrainStorageAsDefault(
            configureOptions: options =>
            {
                options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
            });

        siloBuilder.UseAzureTableReminderService(configureOptions: options =>
        {
            options.Configure(o => o.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;"));
        });

        //Configure the Compliance Service DI inside a Silo running outside ASP.NET 
        siloBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<IComplianceService, ComplianceService>();
        });

        siloBuilder.UseTransactions();

        siloBuilder.AddAzureQueueStreams("StreamProvider", optionsBuilder =>
        {
            optionsBuilder.Configure(options => { options.QueueServiceClient = new QueueServiceClient("UseDevelopmentStorage=true"); });
        })
        .AddAzureTableGrainStorage("PubSubStore", configureOptions: options =>
        {
            options.Configure(o => o.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;"));
        });
    })
    .RunConsoleAsync();