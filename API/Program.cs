using Application;
using Application.Settings;
using Hangfire;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApplication()
                .AddInfrastruture(config)
                .AddEndpointsApiExplorer()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddCors(options => options.AddPolicy("myAppCors", policy =>
                    policy.WithOrigins(config.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                          .AllowAnyHeader()
                          .AllowAnyMethod()));
builder.Services.AddSwaggerGen();
builder.Services.Configure<ProjectSettings>(
    builder.Configuration.GetSection("Project"));

var jwtSettings = config.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "SuperSecretKeyForDevelopmentAndTestingOnly");
if (key.Length < 16)
{
    throw new Exception("JWT Key must be at least 16 bytes (128 bits)!");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
    };
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Handles circular references (e.g., entity navigation properties)
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    // Retains property casing from the DTO/Model names
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseHangfireDashboard("/hangfire");
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("myAppCors");
app.MapControllers();

app.Run();
