using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using DataAccessLayer.Service;
using DataAccessLayer.ViewModel;
using DinkToPdf.Contracts;
using DinkToPdf;
using InvoiceCustomerManagementApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using MongoDbGenericRepository.Utils;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetSection("ConnectionString").Value.ToString();
string databaseName = builder.Configuration.GetSection("DatabaseName").Value.ToString();

string profilePath = builder.Configuration.GetSection("ProfileFilePath").Value.ToString();

// Add services to the container.
builder.Services.AddSingleton<IFileUploadInterface>(serviceProvider =>
{
    return new FileUploadService(profilePath);
});

builder.Services.AddSingleton<ICustomerInterface>(serviceProvider =>
{
    return new CustomerService(connectionString,databaseName);
});

builder.Services.AddSingleton<IItemInterface>(serviceProvider =>
{
    return new ItemService(connectionString, databaseName);
});

builder.Services.AddSingleton<IInvoiceInterface>(serviceProvider =>
{
    return new InvoiceService(connectionString, databaseName);
});

builder.Services.AddSingleton<IMongoCollection<DisplayJson>>(serviceProvider =>
{
    var mongoClient = new MongoClient(connectionString);
    var database = mongoClient.GetDatabase(databaseName);
    return database.GetCollection<DisplayJson>("Invoices");
});

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// identity configration
var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = connectionString,
        DatabaseName = databaseName
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireLowercase = false;
        //lockout
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    }
};
// identity service implement


builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddDefaultTokenProviders();

//BsonSerializer.RegisterSerializer(new StringObjectIdConverter());
// install jwt token
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "https://localhost:5001",
        ValidAudience = "https://localhost:5001",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1swek3u4uo2u4a6e123456789012345678901234567890")),
        ClockSkew = TimeSpan.Zero

    };
});


var app = builder.Build();
//app.UseRotativa();
//RotativaConfiguration.Setup(app.Environment.WebRootPath, @"C:\\Program Files\\wkhtmltopdf\\bin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



