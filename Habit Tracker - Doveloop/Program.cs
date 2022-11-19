using Habit_Tracker___Doveloop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
//cosmos connection
var configuration = builder.Configuration;
try
{
    builder.Services.AddSingleton<ICosmosDbService>(new CosmosDbService(new CosmosClient(configuration["DbConnectionString"]), configuration["DBName"], configuration["HabitLabelContainer"]));
}
catch (Exception e)
{
    try
    {
        builder.Services.AddSingleton<ICosmosDbService>(new CosmosDbService(new CosmosClient(configuration["DbConnectionString2"]), configuration["DBName"], configuration["HabitLabelContainer"]));
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error connecting to database");
    }
}

// Add services to the container.
builder.Services.AddDbContext<CosmosDbContext>(options =>
    options.UseCosmos(configuration["DbConnectionString"], configuration["DBName"]));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CosmosDbContext>();

builder.Services.AddIdentityServer().AddApiAuthorization<IdentityUser, CosmosDbContext>();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"]; 
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
