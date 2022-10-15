using backend.Commons;
using backend.Persistences;
using backend.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using backend.Seeders;
using backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using backend.Hubs;


var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

var settings = builder.Configuration.GetConnectionString("MySqlConnectionApp");
var mailKitOptions = builder.Configuration.GetSection("EmailSettings").Get<MailKitOptions>();
// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddCors(x=>x.AddDefaultPolicy(y=>{
    y.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    y.AllowCredentials().WithOrigins("http://0.0.0.0:4200");
}));
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

builder.Services.AddScoped<ApplicationDbContextInitialiser>();
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddTransient<INotificationService,NotificationService>();
builder.Services.AddMailKit(config => config.UseMailKit(mailKitOptions));
builder.Services.AddSingleton<IUtilityService, UtilityService>();
builder.Services.AddSingleton<IMailHelperService, MailHelperService>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>( c =>
        {
            c.Password.RequireDigit = false;
            c.Password.RequiredLength = 3;
            c.Password.RequiredUniqueChars = 0;
            c.Password.RequireLowercase = false;
            c.Password.RequireUppercase = false;
            c.Password.RequireNonAlphanumeric = false;
            c.User.RequireUniqueEmail = true;
            c.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
    {
        var secret = builder.Configuration["JwtSettings:SymKey"];
        var secretByte = Encoding.UTF8.GetBytes(secret);
        var key = new SymmetricSecurityKey(secretByte);

        c.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key
        };
    })
    .AddJwtBearer("IoTJWTBearer", c =>
    {
        var secret = builder.Configuration["JwtSettings:IoTSymKey"];
        var secretByte = Encoding.UTF8.GetBytes(secret);
        var key = new SymmetricSecurityKey(secretByte);

        c.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            RequireExpirationTime=false
        };
    });
// builder.Services.AddSignalRCore(  o =>{
//                 o.EnableDetailedErrors = true;
//                 o.MaximumReceiveMessageSize = 10240;
//             });

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
        // .UseUrls("http://0.0.0.0:5000/")
        // .UseKestrel()
        // .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    //endpoints.MapControllers();
    endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller}/{action=Index}/{id?}");
    endpoints.MapHub<FarmingHub>("/FarmingHub").AllowAnonymous();
});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");


app.MapFallbackToFile("index.html");;
var t= DateTime.Now;
Console.WriteLine(t.ToString());
app.Run();
