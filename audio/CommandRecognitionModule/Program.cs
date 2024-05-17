using CommandRecognitionModule.Services;

var builder = WebApplication.CreateBuilder(args);

SpeechToCommandService m_service = null;

using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole());

ILogger<SpeechToCommandService> logger = loggerFactory.CreateLogger<SpeechToCommandService>();
m_service = new SpeechToCommandService(logger);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton(m_service);   
var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<SpeechToCommandService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
