using Orleans.Configuration;
using Orleans.Hosting;

var builder = new SiloHostBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "HelloWorldApp";
            });

var host = builder.Build();
await host.StartAsync();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();

await host.StopAsync();
