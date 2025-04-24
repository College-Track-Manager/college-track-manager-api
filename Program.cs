using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CollegeTrackAPI.Data;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.Services;  // Added the namespace for the Email service
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure SQLite and Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Data/college.db"));

// Add Identity with a less strict password policy
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Set the minimum password length
    options.Password.RequiredLength = 6;  // Set to 6 characters instead of higher

    // Disable requirement for a non-alphanumeric character
    options.Password.RequireNonAlphanumeric = false;  // No non-alphanumeric character required

    // Allow lower and upper case without requiring digits
    options.Password.RequireLowercase = true;  // Require lowercase letters
    options.Password.RequireUppercase = true;  // Require uppercase letters

    // Optionally disable the digit requirement
    options.Password.RequireDigit = false;  // Password doesn't require digits
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])
        ),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Set this to ensure User.Identity.Name maps correctly
        NameClaimType = ClaimTypes.Email, // or JwtRegisteredClaimNames.Sub if you prefer
        RoleClaimType = ClaimTypes.Role
    };
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AuditService>();

// Cors configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Email Service for Password Reset (Optional)
// Register the IEmailSender and its implementation EmailSender
builder.Services.AddTransient<CollegeTrackAPI.Services.IEmailSender, EmailSender>();  // This line ensures the email service is available in DI

var app = builder.Build();

// Apply migrations or create DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // db.Database.Migrate(); // Use this if you are using migrations
    //DbInitializer.Seed(db); // Optional if you want to seed the database with initial data

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Student" };

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS middleware
app.UseCors();

// Serve static files (uploads)
//app.UseStaticFiles(); // Enable serving /uploads

// Authentication & Authorization middleware
app.UseAuthentication();  // Use authentication middleware before authorization
app.UseAuthorization();
app.MapControllers();



app.Run();

