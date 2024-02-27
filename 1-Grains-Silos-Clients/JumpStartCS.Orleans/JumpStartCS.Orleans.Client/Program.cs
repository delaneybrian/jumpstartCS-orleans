using JumpStartCS.Orleans.Grains;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient((context, client) =>
{

    client.UseLocalhostClustering(
        gatewayPort: 30001
        );

    client.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "JumpstartCSCluster";
        options.ServiceId = "JumpstartCSService";
    });

});

//builder.Host.UseOrleans(siloBuilder =>
//{
//    siloBuilder.Configure<ClusterOptions>(options =>
//    {
//        options.ClusterId = "JumpstartCSCluster";
//        options.ServiceId = "JumpstartCSService";
//    });

//    siloBuilder.UseLocalhostClustering(
//        siloPort: 30002, gatewayPort: 30001
//        );
//});

var app = builder.Build();


app.MapGet("/", async (IClusterClient clusterClient) => {

    var customerId = Guid.NewGuid().ToString();

    var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

    var balance = await customerGrain.GetCustomerCheckingAccountBalance();

    return balance;

});

app.Run();
