using Microsoft.EntityFrameworkCore;
using NLog.Web;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Domain.Contracts;
using Infrastructure;
using Infrastructure.Repositories;
using Application.Services;
using Application.Mappings;
using PostApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Description = "Enter 'Bearer' followed by a space and your JWT token"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });
    builder.Services.AddSwaggerGen();

    // rejestracja automappera w kontenerze IoC
    builder.Services.AddAutoMapper(typeof(PostMappingProfile));

    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"], // Issuer z IdentityService
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"], // Audience z IdentityService
                ValidateLifetime = true, // Sprawdza czas wa¿noœci tokenu
                ValidateIssuerSigningKey = true, // W³¹czenie weryfikacji klucza podpisuj¹cego
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
            };
        });


    // rejestracja automatycznej walidacji (FluentValidation waliduje i przekazuje wynik przez ModelState)
    //builder.Services.AddFluentValidationAutoValidation();

    // rejestracja kontekstu bazy w kontenerze IoC
    //var sqliteConnectionString = @"Data Source=Kiosk.WebAPI.Logger.db";
    // var sqliteConnectionString = @"Data Source=c:\DyskD\SaleKioskStudent.db";
    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<PostDbContext>(options =>
        options.UseSqlServer(mssqlConnectionString));

    // rejestracja walidatora

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IPostUnitOfWork, PostUnitOfWork>();

    builder.Services.AddScoped<IPostRepository, PostRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<ILikeRepository, LikeRepository>();

    builder.Services.AddScoped<DataSeeder>();

    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<ILikeService, LikeService>();

    builder.Services.AddScoped<ExceptionMiddleware>();

    // rejestruje w kontenerze zale¿noœci politykê CORS o nazwie SaleKiosk,
    // która zapewnia dostêp do API z dowolnego miejsca oraz przy pomocy dowolnej metody
    builder.Services.AddCors(o => o.AddPolicy("TTSAW", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseStaticFiles();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // wstawia politykê CORS obs³ugi do potoku ¿¹dania
    app.UseCors("TTSAW");

    // seeding data
    using (var scope = app.Services.CreateScope())
    {
        var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        dataSeeder.Seed();
    }

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}







