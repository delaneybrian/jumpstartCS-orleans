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

        siloBuilder.UseLocalhostClustering(
            siloPort: 30000, gatewayPort: 30001
            ); 
    })
    .RunConsoleAsync();