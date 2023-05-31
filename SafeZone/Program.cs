using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SafeZone.Data;
using SafeZone.Hubs;
using SafeZone.Repositories;
using SafeZone.Services;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Disable property name conversion
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("token", new OpenApiSecurityScheme
    {
        Description = "Standart Authorization Header /Bearer {token}",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "token"
        },
        In = ParameterLocation.Header
    };
    //var requirements = new OpenApiSecurityRequirement
    //{
    //    {scheme,new List<string>() }
    //};
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Token")))
        };
    });


builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionstring = builder.Configuration.GetConnectionString("Default");
    options.UseMySql(
        connectionstring,
        ServerVersion.AutoDetect(connectionstring));
});


builder.Services.AddScoped<IEmailRepository, GoogleEmailService>();
builder.Services.AddScoped<IUserRepository, UserService>();
builder.Services.AddScoped<ChatHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");
app.MapHub<CrimeHub>("/crime");

app.Run();
