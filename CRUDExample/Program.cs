using Services;
using ServiceContracts;
using Enttities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();



//add services into IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

//service za di dbContext i UseSqlServer za db connection
builder.Services.AddDbContext<PersonsDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup(rootPath:"wwwroot", wkhtmltopdfRelativePath:"Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();



app.Run();
