using Services;
using ServiceContracts;
using Enttities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Logging
//builder.Host.ConfigureLogging(loggingProvider => 
//{
//    loggingProvider.ClearProviders();
//    // dodajemo logging provider koji zelimo exoplicitno
//    loggingProvider.AddConsole(); 
//    loggingProvider.AddDebug();
//    loggingProvider.AddEventLog();
//});

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    //read configuration settings from built -in IConfiguration
    .ReadFrom.Configuration(context.Configuration)
    //read out current app services and make them available to serilog
    .ReadFrom.Services(services);
});


builder.Services.AddControllersWithViews();


//add services into IoC container
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

//service za di dbContext i UseSqlServer za db connection
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False

builder.Services.AddHttpLogging(opt =>
{
    opt.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();


app.UseSerilogRequestLogging();

//create application pipeline
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//http logging
app.UseHttpLogging();

/*
app.Logger.LogDebug("debug-message");
app.Logger.LogInformation("information-message");
app.Logger.LogWarning("warning-message");
app.Logger.LogError("error-message");
app.Logger.LogCritical("critical-message");
*/

if (builder.Environment.IsEnvironment("Test")==false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup(rootPath:"wwwroot", wkhtmltopdfRelativePath:"Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();



app.Run();

//make the auto-generated Program accessible programatically
public partial class Program { }
