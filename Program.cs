using Autofac.Core;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Middleware;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Configuration;
using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);
;
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
    options.User.RequireUniqueEmail = false;
});
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISubscriptionContext, SubscriptionService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    options.AddPolicy("AllowSpecificOrigins",
            builder =>
            {
                builder.WithOrigins("https://localhost:44352", "https://localhost:44352/Authorization/FacebookSignIn") // Replace {URL} with your actual redirect URL
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
});
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHostedService<TasksBgService>();
builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession();
Log.Logger = new LoggerConfiguration()
       .WriteTo.File("Logs/mylog.txt", rollingInterval: RollingInterval.Day)
       .CreateLogger();
builder.Services.AddLogging(builder =>
{
    builder.AddSerilog();
});

builder.Services.AddSingleton(
    new SmtpClient
    {
        Host = smtpSettings.Server,
        Port = smtpSettings.Port,
        Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
        EnableSsl = true
    }
    );

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
AppResources.AssignResources();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseMiddleware<BlockRegistrationMiddleware>();
app.UseCors("AllowAll");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "Default_Posts",
        "{controller}/{action}/{id?}",
        new { area = "", controller = "Home", action = "Welcome" }
    );
    endpoints.MapControllerRoute(
        "Default_Posts",
        "Subscriptions/GetById/{id?}",
        new { area = "", controller = "Home", action = "Welcome" }
    );
    endpoints.MapControllerRoute(
        "myRoute",
        "Posts/Publish/{postId?}",
        new { controller = "Posts", action = "Publish" }
    );
    endpoints.MapAreaControllerRoute(
        "SocialRoutes",
        "Social",
        "Social/{controller=SocialHome}/{action=Index}/{id?}",
        new { controller = "SocialHome", action = "Index" }
    );
    endpoints.MapAreaControllerRoute(
       "SocialShare",
       "Social",
       "Social/{controller=Publish}/{action=Share}"
   );
    endpoints.MapControllerRoute(
        "postInfo",
        "Posts/PostInfo/{Id?}",
        new { controller = "Posts", action = "Publish" }
    );

    endpoints.MapControllerRoute(
        "UpdatePost",
        "{controller = Posts}/{action = UpdatePost}/{id?}",
        new { controller = "Posts", action = "UpdatePost" }
    );
    endpoints.MapControllerRoute(
        "TechConfigs",
        "{controller = Configs}/{action = TechnicalConfigs}"
    );

    endpoints.MapControllerRoute(
        "RegisterNewUser",
        "{controller = User}/{action = Register}/{id?}",
        new { controller = "Posts", action = "UpdatePost" }
    );
    endpoints.MapControllerRoute(
        "Channels",
        "{area = Social}/{controller = Channels}/{action = List}",
        new { area = "Social" , controller = "Channels", action = "List" }
    );
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "areas",
        "{area:exists}/{controller=Home}/{action=Welcome}/{id?}"
    );
});

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Welcome}/{Post}",
    new { controller = "Home", action = "Welcome" });


app.MapRazorPages();

app.Run();