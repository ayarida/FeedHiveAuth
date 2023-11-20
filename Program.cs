using FeedHiveAuth.Data;
using FeedHiveAuth.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

/*
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();*/

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISubscriptionContext, SubscriptionService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Editor", policy => policy.RequireRole("Editor"));
    options.AddPolicy("Viewer", policy => policy.RequireRole("Viewer"));
});
builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Editor", "Viewer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
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
        name: "Default_Posts",
        pattern: "{controller}/{action}/{id?}",
        defaults: new { area = "", controller = "Home", action = "Index" }
     );
    endpoints.MapControllerRoute(
        name: "Default_Posts",
        pattern: "Subscriptions/GetById/{id?}",
        defaults: new { area = "", controller = "Home", action = "Index" }
     );
    endpoints.MapControllerRoute(
       name: "myRoute",
       pattern: "Posts/Publish/{postId?}",
       defaults: new { controller = "Posts", action = "Publish" }
   );
    endpoints.MapAreaControllerRoute(
        name: "SocialRoutes",
        areaName:"Social",
        pattern: "Social/{controller=SocialHome}/{action=Index}/{id?}",
        defaults: new { controller = "SocialHome", action = "Index" }
        );
    endpoints.MapControllerRoute(
      name: "postInfo",
      pattern: "Posts/PostInfo/{Id?}",
      defaults: new { controller = "Posts", action = "Publish" }
  );

    endpoints.MapControllerRoute(
     name: "UpdatePost",
     pattern: "{controller = Posts}/{action = UpdatePost}/{id?}",
     defaults: new { controller = "Posts", action = "UpdatePost" }
     );

      endpoints.MapControllerRoute(
     name: "RegisterNewUser",
     pattern: "{controller = User}/{action = Register}/{id?}",
     defaults: new { controller = "Posts", action = "UpdatePost" }

  );

});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{Post}");

app.MapRazorPages();

app.Run();
