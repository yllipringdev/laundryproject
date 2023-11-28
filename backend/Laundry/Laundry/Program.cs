using Laundry.Data;
using Laundry.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
builder.Services.AddDbContext<Context>(options =>
    options.UseMySql(
        mySqlConnectionStringBuilder.ConnectionString,
        ServerVersion.AutoDetect(mySqlConnectionStringBuilder.ConnectionString)),
    ServiceLifetime.Scoped);

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Ensure unique email addresses
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<Context>();

// Configure JWT authentication


var jwtIssuer = builder.Configuration.GetSection("JWT:ValidIssuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("JWT:SecretKey").Get<string>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();

// Enable CORS
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var serviceProvider = serviceScope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await ApplicationDbContextSeed.InitializeAsync(serviceProvider, userManager, roleManager);
}

app.Run();
