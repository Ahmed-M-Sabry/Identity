using Identity.Data;
using Identity.Models;
using UserMangmentService.Models;
using UserMangmentService.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MailKit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddTransient<IEmailServices, EmailServices>();

builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
