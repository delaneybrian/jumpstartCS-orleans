using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient((context, client) => {

    client.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "my-first-cluster";
        options.ServiceId = "MyOrleansService";
    });

});
   
var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.Run();
