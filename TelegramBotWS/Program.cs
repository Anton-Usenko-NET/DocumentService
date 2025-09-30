using TelegramBotWS;
using Serilog;

string applicationDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
var dotenv = Path.Combine(applicationDirectory, ".env");
Directory.SetCurrentDirectory(applicationDirectory);
DotEnv.Load(dotenv);


var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, configuration) => configuration
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    )
    .UseWindowsService(options => {
        options.ServiceName = "TelegramBotWS";
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
