

//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//var app = builder.Build();

//app.UseRouting();
//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();
//app.AddDistributedMemoryCache();

//app.UseAuthorization();
//app.UseSession();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Register}/{id?}");


//app.Run();













using Microsoft.AspNetCore.HttpOverrides;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//builder.Services.AddScoped<ScreenAccessService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Add HSTS for production
}

// Use forwarded headers to handle reverse proxy scenarios
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();
app.UseStaticFiles();




// Configure request routing
app.UseRouting();

// Enable session middleware
app.UseSession();
// Authorization middleware
app.UseAuthorization();

// Map routes
app.MapControllers();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Register}/{id?}");

app.MapControllerRoute(
    name: "EncryptedRoute",
    pattern: "{encryptedData}",
    defaults: new { controller = "Employee", action = "List" }
);

//var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
//XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
// Bind application to the configured hosting port
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var hostingPort = config["HostingPort"];
if (!string.IsNullOrEmpty(hostingPort) && !app.Environment.IsDevelopment())
{
    app.Urls.Add(hostingPort);
}

app.Run();