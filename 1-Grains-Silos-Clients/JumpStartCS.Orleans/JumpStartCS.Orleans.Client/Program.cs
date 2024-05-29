using JumpStartCS.Orleans.Client.Contracts;
using JumpStartCS.Orleans.Grains;
using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Infrastructure;
using Microsoft.CodeAnalysis.Differencing;
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

    client.UseTransactions();
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
    IClusterClient clusterClient,
    ITransactionClient transactionClinet) => {

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        decimal balance = 0;
        await transactionClinet.RunTransaction(TransactionOption.Create, async () =>
        {
            balance = await checkingAccountGrain.GetBalance();
        });

        return TypedResults.Ok(balance);
    });

app.MapPost("checkingaccount", async (
    CreateAccount createAccount,
    IClusterClient clusterClient,
    ITransactionClient transactionClinet) => {

        var checkingAccountId = Guid.NewGuid();

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        await transactionClinet.RunTransaction(TransactionOption.Create, async () =>
        {
            await checkingAccountGrain.Initialise(createAccount.OpeningBalance, createAccount.CustomerId); 
        });

        return TypedResults.Created($"checkingAccount/{checkingAccountId}", checkingAccountId);
    });

app.MapPost("checkingaccount/{checkingAccountId}/debit", async (
    Guid checkingAccountId,
    Debit debit,
    IClusterClient clusterClient,
    ITransactionClient transactionClinet) => {

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        await transactionClinet.RunTransaction(TransactionOption.Create, async () =>
        {
            await checkingAccountGrain.Debit(debit.Amount);
        });

        return TypedResults.NoContent();
    });

app.MapPost("checkingaccount/{checkingAccountId}/credit", async (
    Guid checkingAccountId, 
    Credit credit, 
    IClusterClient clusterClient,
    ITransactionClient transactionClinet) => {

        var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(checkingAccountId);

        await transactionClinet.RunTransaction(TransactionOption.Create, async () =>
        {
            await checkingAccountGrain.Credit(credit.Amount);
        });

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
    IClusterClient clusterClient,
    ITransactionClient transactionClinet) =>
{
    var atmId = Guid.NewGuid();

    var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);

    await transactionClinet.RunTransaction(TransactionOption.Create, async () =>
    {
        await atmGrain.Initialise(createAtm.OpeningBalance);
    });

    return TypedResults.Created($"atm/{atmId}", atmId);
});

app.MapPost("atm/{atmId}/withdrawl", async (
    Guid atmId,
    AtmWithdrawl atmWithdrawl,
    IClusterClient clusterClient,
    ITransactionClient transactionClinet) =>
{
    var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);
    var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(atmWithdrawl.CheckingAccountId);

    await transactionClinet.RunTransaction(TransactionOption.Create,
        async () =>
        {
            await atmGrain.Withdraw(atmWithdrawl.WithdrawlAmount);
            await checkingAccountGrain.Debit(atmWithdrawl.WithdrawlAmount);
        });

    return TypedResults.NoContent();
});

app.Run();
