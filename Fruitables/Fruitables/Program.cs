using Fruitables.Models;
using Fruitables.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var cs = "Server=DESKTOP-FPOIVNS; Initial Catalog= Fruitables; User ID=sa; Password=aptech; TrustServerCertificate=True";
builder.Services.AddDbContext<MainDbContextFile>(a => a.UseSqlServer(cs));
builder.Services.AddSession();
builder.Services.AddTransient<EmailService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();
