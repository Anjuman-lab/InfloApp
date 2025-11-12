using System;
using Microsoft.AspNetCore.Builder;        
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagement.Data;
using Westwind.AspNetCore.Markdown;


var builder = WebApplication.CreateBuilder(args);

// Read connection string from appsettings.json
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// Register EF Core with SQL Server
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(cs));


// Register abstractions for DI
builder.Services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());
builder.Services.AddDomainServices();

// UI frameworks
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppBaseUrl"] ?? "https://localhost:7084/");
});
builder.Services.AddMarkdown();


var app = builder.Build();

// Middleware Pipeline
if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseMarkdown();

// Endpoints
app.MapControllers();
app.MapRazorPages();      
app.MapBlazorHub();       
app.MapFallbackToPage("/_Host"); // ⬅ fallback to Blazor at root


app.Run();
