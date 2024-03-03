using JumpStartCS.Orleans.Grains;
using JumpStartCS.Orleans.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient((context, client) =>
{

    client.UseLocalhostClustering(gatewayPort: 30001);

    client.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "JumpstartCSCluster";
        options.ServiceId = "JumpstartCSService";
    });

});

//Add if we want to use analytics service in a ASP.NET hosted silo
//builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();

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

app.MapGet("customers/{customerId}", async (
    string customerId,
    IClusterClient clusterClient) => {

        var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

        var details = await customerGrain.GetCustomerDetails();

        return TypedResults.Ok(details);
    });

app.MapGet("customers/{customerId}/accounts/{accountId}", async (
    string customerId, 
    Guid accountId, 
    IClusterClient clusterClient) => {

    var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

    var balance = await customerGrain.GetCustomerCheckingAccountBalance(accountId);

    return TypedResults.Ok(balance);

});

app.MapPost("customers/{customerId}/starttimer", async(
    [FromBody] string timerName,
    string customerId,
    IClusterClient clusterClient) => {

        var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

        await customerGrain.StartTimer(timerName);
    });

app.MapPost("customers/{customerId}/startReminder", async (
    [FromBody] string reminderName,
    string customerId,
    IClusterClient clusterClient) => {

        var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

        await customerGrain.StartReminder(reminderName);
    });

app.MapPost("customers/{customerId}", async (
    [FromBody] string name,
    string customerId,
    IClusterClient clusterClient) => {

        var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

        await customerGrain.AddCustomerDetails(name);
    });

app.MapPost("customers/{customerId}/accounts/{accountId}", async (
    [FromBody] int debitAmount,
    string customerId,
    Guid accountId,
    IClusterClient clusterClient) => {

        var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

        await customerGrain.DebitAccount(accountId, debitAmount);
    });

app.Run();
