using FeedHiveAuth.Data;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
;
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISubscriptionContext, SubscriptionService>();

builder.Services.AddHttpContextAccessor();
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

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Editor", "Viewer" };

    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

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
app.UseSession();
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