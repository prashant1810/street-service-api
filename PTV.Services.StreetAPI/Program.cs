using AutoMapper;
using Street.Services.CrudAPI;
using Street.Services.CrudAPI.Database;
using Street.Services.CrudAPI.Extensions;
using PTV.Services.StreetAPI.Services.Interfaces;
using PTV.Services.StreetAPI.Services.Repository;
using PTV.Services.StreetAPI.Extensions;
using NetTopologySuite.Geometries;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using StackExchange.Redis;
using RedLockNet;
using Serilog;
using FluentAssertions.Common;
using WeightCalculator.JsonConfigData;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);
var multiplexer = new List<RedLockMultiplexer> {
    ConnectionMultiplexer.Connect("street-cache:6379")
};
var redlockFactory = RedLockFactory.Create(multiplexer);
builder.Services.AddSerilog(options =>
{
    //Override Few of the Configurations
    options.Enrich.WithProperty("Application", "StreetAPI")
       .Enrich.WithProperty("Environment", "Dev")
       .WriteTo.Console(new RenderedCompactJsonFormatter())
       .WriteTo.GrafanaLoki("http://loki:3100");
});
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.
builder.Services.AddSingleton<GeometryFactory>();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IStreetService, StreetService>();
builder.Services.AddScoped<IGeometryService, GeometryService>();
builder.Services.AddSingleton<IDistributedLockFactory>(redlockFactory);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new LineStringJsonConverter());
    });

JsonConfigLoader.Initialize();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    MigrationExtensions.ApplyMigrations(app);
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();

public partial class Program { }