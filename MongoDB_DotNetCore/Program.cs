using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MongoDB_DotNetCore.Controllers.MongoDB;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Domain;
using MongoDB_DotNetCore.Domain.Stream;
using System.Timers;

var builder = WebApplication.CreateBuilder(args);


// Add SQL services to the container.
builder.Services.AddDbContext<SQLDatabaseContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SqlServer")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<MongoDbContext>();

// for store in database

builder.Services.AddScoped<L1Signal_Pool_Migration>(); // Scoped because it uses DbContext
builder.Services.AddScoped<Alarm_History_Migration>(); // Scoped because it uses DbContext
builder.Services.AddScoped<L1_Pool_Migration>(); // Scoped because it uses DbContext
builder.Services.AddScoped<Tag_Pool_Migration>(); // Scoped because it uses DbContext
builder.Services.AddScoped<L1Signal_Pool_Capped_Migration>(); // Scoped because it uses DbContext
builder.Services.AddScoped<Operator_History_Transfer>(); 
builder.Services.AddScoped<ProductResult_History_Transfer>();

// for hub streaming
builder.Services.AddScoped<L1_Pool_Opened_Transfer>(); 
builder.Services.AddScoped<L1Signal_Pool_Active_Transfer>();
builder.Services.AddScoped<Operator_History_Active_Transfer>();
builder.Services.AddScoped<ProductResult_History_Active_Transfer>();
builder.Services.AddScoped<Tag_Pool_Active_Transfer>();


// Register the timer service
//builder.Services.AddSingleton<MyTimerService>();
builder.Services.AddScoped<MyTimerService>(); // Scoped version

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//// Start the timer service on application start
//var timerService = app.Services.GetRequiredService<MyTimerService>();
//timerService.Start
//

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SQLDatabaseContext>();
    db.Database.Migrate();
}

MessageHubConnection hubConnection = new("https://enscloud.in/mh", "MessageHub");
hubConnection.Connect();

using (var scope = app.Services.CreateScope())
{
    var timerService = scope.ServiceProvider.GetRequiredService<MyTimerService>();
    timerService.HubConnection = hubConnection;
    timerService.Start();
}



app.Run();


// Timer Service Class
public class MyTimerService
{
    private readonly System.Timers.Timer _timer;
    private bool Inprocess = false;
    //private readonly mongoToSqlL1Signal_PoolMigration _mongoToSqlL1Signal_PoolMigration;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // Time when the timer should trigger
    private DateTimeOffset timeToProcess;

    public MessageHubConnection? HubConnection { get; set; } = null;

    public MyTimerService(L1Signal_Pool_Migration mongoToSqlMigration, IServiceScopeFactory serviceScopeFactory)
    {
        //_mongoToSqlL1Signal_PoolMigration = mongoToSqlMigration;
        _serviceScopeFactory = serviceScopeFactory;
        _timer = new System.Timers.Timer(5000); //  seconds interval
        //_timer.Elapsed += OnTimedEvent;
        _timer.Elapsed += async (sender, e) => await HandleTimer();
        _timer.AutoReset = true;

        // ============================== Initialize time to the input time for Test
        //string input = "2019-11-09T05:06:28.000+00:00";
        var inputUtcDate = DateTime.ParseExact(
            "2019-09-04T10:49:55.500+00:00", // test date - 4 min
            "yyyy-MM-ddTHH:mm:ss.fffK",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AdjustToUniversal
        );
        //string input = "2019-09-04T10:53:54.500+00:00";
        string input = inputUtcDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //2019 - 09 - 23T10: 27:44.500 + 00:00

        timeToProcess = DateTimeOffset.Parse(input);
    }

    public void Start()
    {
        _timer.Start();
        Console.WriteLine("Timer started.");
    }

    //private void OnTimedEvent(object sender, ElapsedEventArgs e)
    //{
       
    //    Console.WriteLine($"Timer elapsed at {e.SignalTime}");
    //    // Add your logic here, e.g., database operations or service calls


    //}

    private async Task HandleTimer()
    {
        if (Inprocess)
        {
            //Console.WriteLine("Skipping execution as another process is running.");
            return;
        }

        Inprocess = true;

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var L1Signal_Pool_Migration = scope.ServiceProvider.GetRequiredService<L1Signal_Pool_Migration>();
                var Alarm_History_Migration = scope.ServiceProvider.GetRequiredService<Alarm_History_Migration>();
                var L1_Pool_Migration = scope.ServiceProvider.GetRequiredService<L1_Pool_Migration>();
                var Tag_Pool_Migration = scope.ServiceProvider.GetRequiredService<Tag_Pool_Migration>();
                var L1Signal_Pool_Capped_Migration = scope.ServiceProvider.GetRequiredService<L1Signal_Pool_Capped_Migration>();
                var Operator_History_Transfer = scope.ServiceProvider.GetRequiredService<Operator_History_Transfer>();
                var ProductResult_History_Transfer = scope.ServiceProvider.GetRequiredService<ProductResult_History_Transfer>();

                // for Msg hub streaming

                var L1_Pool_Opened_Transfer = scope.ServiceProvider.GetRequiredService<L1_Pool_Opened_Transfer>();
                var L1Signal_Pool_Active_Transfer = scope.ServiceProvider.GetRequiredService<L1Signal_Pool_Active_Transfer>();
                var Operator_History_Active_Transfer = scope.ServiceProvider.GetRequiredService<Operator_History_Active_Transfer>();
                var ProductResult_History_Active_Transfer = scope.ServiceProvider.GetRequiredService<ProductResult_History_Active_Transfer>();
                var Tag_Pool_Active_Transfer = scope.ServiceProvider.GetRequiredService<Tag_Pool_Active_Transfer>();




                //========================== for Test
                timeToProcess = timeToProcess.AddMinutes(4);
                Console.WriteLine($"Updated time: {timeToProcess}");

                // Call the GetRecentRecords method

                //await L1Signal_Pool_Migration.addL1Signal_PoolRecentRecords(timeToProcess);
                //await Alarm_History_Migration.addAlarm_HistoryRecentRecords(timeToProcess);
                //await L1_Pool_Migration.addL1_PoolRecentRecords(timeToProcess);
                //await Tag_Pool_Migration.addTag_PoolRecentRecords(timeToProcess);
                //await L1Signal_Pool_Capped_Migration.addL1Signal_Pool_CappedRecentRecords(timeToProcess);
                //await Operator_History_Transfer.addOperator_HistoryRecentRecords(timeToProcess);
                //await ProductResult_History_Transfer.addProductResult_HistoryRecentRecords(timeToProcess);

                //// for Msg hub streaming
                await Tag_Pool_Active_Transfer.transferTag_Pool_ActiveRecentRecords(HubConnection!);
                await L1Signal_Pool_Active_Transfer.transferL1Signal_Pool_ActiveRecentRecords(HubConnection!);
                await Operator_History_Active_Transfer.transferOperator_History_ActiveRecentRecords(HubConnection!);
                await ProductResult_History_Active_Transfer.transferProductResult_History_ActiveRecentRecords(HubConnection!);
                await L1_Pool_Opened_Transfer.transferL1_Pool_OpenedRecentRecords(HubConnection!);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnTimedEvent: {ex.Message}");
        }
        finally
        {
            Inprocess = false;
        }
    }

    public void Stop()
    {
        _timer.Stop();
        Console.WriteLine("Timer stopped.");
    }
}
