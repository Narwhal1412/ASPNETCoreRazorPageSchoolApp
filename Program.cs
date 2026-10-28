using Microsoft.EntityFrameworkCore;
using SchoolAppCore.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<SchoolContenxt>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolContenxt") ?? throw new InvalidOperationException("Connection string 'SchoolContenxt' not found.")));

// Connection to the database
builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDBConnectionString")));

var app = builder.Build();

// Absolute path for code-server prxy
// if (app.Environment.IsDevelopment())
// {
//     app.UsePathBase("/absproxy/5000");
// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();