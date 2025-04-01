using DotNetEnv; // Import the package
using System.Text;
using identity.interfaces;
using identity.Repository;
using identity.Service;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();


string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(connectionString); ; //setting db connection at initialization
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
    //This is to enable annotations in the swagger UI. This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models.
    //This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models. This is important for the swagger UI to be able to display the annotations in the controllers and models.
    option.EnableAnnotations();
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ApplicationDBContext>();

// Add Jwt Authentication to the service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_Issuer"),
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_Audience"),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SigningKey")))
    };
});

// Add some custom services to the service container
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IStockRepository, StockRepo>();
builder.Services.AddScoped<IStockRepository, StockRepo>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepo>();
builder.Services.AddScoped<ICommentRepository, CommentRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


//Adding cors policy to allow requests from all origins, headers and methods
//This is important for the frontend to be able to communicate with the backend
//You can also specify the allowed origins, headers and methods if you want to restrict the requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity v1"));
}

app.UseHttpsRedirection();

// Use Jwt for Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



