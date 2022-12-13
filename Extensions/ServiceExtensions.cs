using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HeiLiving.Quotes.Api.Services;
using HeiLiving.Quotes.Api.Repositories;
using HeiLiving.Quotes.Api.Models;
using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using HeiLiving.Quotes.Api.Auth;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.EnvironmentName.Equals("Development"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(configuration.GetConnectionString("DefaultConnection")));
        }
        else
        {
            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }

    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IJwtFactory, JwtFactory>();
        services.AddSingleton<IAuthorizationCodeFactory, AuthorizationCodeFactory>();

        services.AddScoped<ICondoHotelExpensesRepository, CondoHotelExpensesRepository>();
        services.AddScoped<IExpensesRepository, ExpensesRepository>();
        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IRatesRepository, RatesRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IStagesRepository, StagesRepository>();
        services.AddScoped<ITemporalitiesRepository, TemporalitiesRepository>();
        services.AddScoped<ITradePoliciesRepository, TradePoliciesRepository>();
        services.AddScoped<IUnitsRepository, UnitsRepository>();
        services.AddScoped<IUnitEquipmentsRepository, UnitEquipmentsRepository>();
        services.AddScoped<IUnitRatesRepository, UnitRatesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        services.AddScoped<IAccountsService, AccountsService>();
        services.AddScoped<IQuotesService, QuotesService>();
        services.AddScoped<ICondoHotelExpensesService, CondoHotelExpensesService>();
        services.AddScoped<IExpensesService, ExpensesService>();        
        services.AddScoped<IProjectsService, ProjectsService>();
        services.AddScoped<IRatesService, RatesService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IStageService, StageService>();
        services.AddScoped<ITemporalitiesService, TemporalitiesService>();
        services.AddScoped<ITradePoliciesService, TradePoliciesService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IUnitEquipmentsService, UnitEquipmentsService>();
        services.AddScoped<IUnitRatesService, UnitRatesService>();
        services.AddScoped<IUserService, UserService>();
    }

    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // jwt wire up
        // Get options from app settings
        var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

        var secret = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.SecretKey)]));

        // Configure JwtIssuerOptions
        services.Configure<JwtIssuerOptions>(options =>
        {
            options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
            options.SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        });

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

            ValidateAudience = true,
            ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = secret,

            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(configureOptions =>
        {
            configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            configureOptions.RequireHttpsMetadata = false;
            configureOptions.SaveToken = true;
            configureOptions.TokenValidationParameters = tokenValidationParameters;
        });
    }
}