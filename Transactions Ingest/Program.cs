using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Transactions_Ingest.Data;
using Transactions_Ingest.Services;


//Ihost service to create DI container
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "transactions.db");
        var connectionString = $"Data Source={dbPath}";

        // get connection string from appsettings.json automatically
        services.AddDbContext<TransactionDbContext>(options =>
            options.UseSqlite(connectionString));

       
        // MockApi — reads FilePath from appsettings.json
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), hostContext.Configuration["Api:FilePath"] ?? "data.json");
        services.AddSingleton(new MockApi(filePath));

        // IngestionLogic
        services.AddScoped<IngestionLogic>();
    })
    .Build();

// Apply migrations
using var scope = host.Services.CreateScope();


    var db = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    Console.WriteLine(db.Database.GetDbConnection().DataSource);
    await db.Database.MigrateAsync();

    // Run ingestion logic
    var service = scope.ServiceProvider.GetRequiredService<IngestionLogic>();
    await service.RunAsync();

//logic to print the transcations on screen

Console.WriteLine("\n=================================================================");

var transactions = await db.Transactions.ToListAsync();
Console.WriteLine($"\nTotal Transactions: {transactions.Count}");
Console.WriteLine($"{"ID",-5} {"Card",-6} {"Location",-10} {"Product",-15} {"Amount",-10} {"Status",-12} {"Time"}");
Console.WriteLine(new string('-', 80));

foreach (var t in transactions)
{
    Console.WriteLine($"{t.TransactionId,-5} {t.CardNumberLast4,-6} {t.LocationCode,-10} {t.ProductName,-15} {t.Amount,-10} {t.Status,-12} {t.TransactionTime}");
}

// Show all audits
var audits = await db.Audits.ToListAsync();
Console.WriteLine($"\nTotal Audit Logs: {audits.Count}");
Console.WriteLine($"{"ID",-5} {"TxnID",-7} {"Field",-15} {"OldValue",-15} {"NewValue",-15} {"ModifiedAt"}");
Console.WriteLine(new string('-', 80));

foreach (var a in audits)
{
    Console.WriteLine($"{a.Id,-5} {a.TransactionId,-7} {a.Name,-15} {a.OldValue,-15} {a.NewValue,-15} {a.ModifiedAt}");
}

Console.WriteLine("\nTransaction Ingestion finished");


