using JumpStartCS.Orleans.Client.Contracts;
using JumpStartCS.Orleans.Grains;
using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Infrastructure;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient((context, client) =>
{
    client.UseAzureStorageClustering(configureOptions: options =>
    {
        options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
    });

    client.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "JumpstartCSCluster";
        options.ServiceId = "JumpstartCSService";
    });
});

//Add if we want to use analytics service in a ASP.NET hosted silo
builder.Services.AddSingleton<IComplianceService, ComplianceService>();

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

app.MapGet("checkingaccount/{checkingAccountId}/balance", async (
    Guid checkingAccountId,
    IClusterClient clusterClient) => {

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        var balance = await checkingAccountGrain.GetBalance();

        return TypedResults.Ok(balance);
    });

app.MapPost("checkingaccount", async (
    CreateAccount createAccount,
    IClusterClient clusterClient) => {

        var checkingAccountId = Guid.NewGuid();

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        await checkingAccountGrain.Initialise(createAccount.OpeningBalance, createAccount.CustomerId);

        return TypedResults.Created($"checkingAccount/{checkingAccountId}", checkingAccountId);
    });

app.MapPost("checkingaccount/{checkingAccountId}/debit", async (
    Guid checkingAccountId,
    Debit debit,
    IClusterClient clusterClient) => {

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        await checkingAccountGrain.Debit(debit.Amount);

        return TypedResults.NoContent();
    });

app.MapPost("checkingaccount/{checkingAccountId}/credit", async (
    Guid checkingAccountId, 
    Credit credit, 
    IClusterClient clusterClient) => {

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        await checkingAccountGrain.Credit(credit.Amount);

        return TypedResults.NoContent();
});

app.MapPost("checkingaccount/{checkingAccountId}/reccuringPayment", async (
    Guid checkingAccountId,
    CreateRecurringPayment createRecurringPayment,
    IClusterClient clusterClient) => {

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        await checkingAccountGrain.ScheduleRecurringPayment(
            createRecurringPayment.Id, 
            createRecurringPayment.Amount, 
            createRecurringPayment.ReccursEveryMinute);

        return TypedResults.NoContent();
    });

app.MapGet("atm/{atmId}/balance", async (
    Guid atmId,
    IClusterClient clusterClient) =>
{
    var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);

    var balance = await atmGrain.Balance();

    return TypedResults.Ok(balance);
});

app.MapPost("atm", async (
    CreateAtm createAtm,
    IClusterClient clusterClient) =>
{
    var atmId = Guid.NewGuid();

    var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);

    await atmGrain.Initialise(createAtm.OpeningBalance);

    return TypedResults.Created($"atm/{atmId}", atmId);
});

app.MapPost("atm/{atmId}/withdrawl", async (
    Guid atmId,
    AtmWithdrawl atmWithdrawl,
    IClusterClient clusterClient) =>
{
    var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);

    await atmGrain.Withdraw(atmWithdrawl.CheckingAccountId, atmWithdrawl.WithdrawlAmount);

    return TypedResults.NoContent();
});

app.Run();
