using backend.Commons;
using backend.Persistences;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetConnectionString("MySqlConnectionApp");
var mailKitOptions = builder.Configuration.GetSection("EmailSettings").Get<MailKitOptions>();
// Add services to the container.

builder.Services.AddControllersWithViews().AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//services.AddSingleton<ICurrentUserService, CurrentUserService>();
//services.AddSingleton<ICurrentUserService, CurrentUsesrService>();
builder.Services.AddScoped<IDateTime, DateTimeService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICurrentIoTService, CurrentIoTService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(settings, new MySqlServerVersion(new Version(10, 1, 40))));
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddTransient<INotificationService,NotificationService>();
builder.Services.AddMailKit(config => config.UseMailKit(mailKitOptions));
builder.Services.AddSingleton<IUtilityService, UtilityService>();
builder.Services.AddSingleton<IMailHelperService, MailHelperService>();
builder.Services.AddSingleton<IMailTemplateHelperService, MailTemplateHelperService>();
//services.AddScoped<IHttpClientFactory>();
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// builder.Services.AddHealthChecks()
//     .AddDbContextCheck<AppIdentityDbContext>("App Identity")
//     .AddDbContextCheck<ApplicationDbContext>("App Db");

// builder.Services.AddControllersWithViews(options =>
//     options.Filters.Add<ApiExceptionFilterAttribute>())
//         .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");;

app.Run();
