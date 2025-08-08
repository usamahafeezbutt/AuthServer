using AuthServer.API.Services;
using AuthServer.Application.Common.Interfaces.ContentFiles;
using AuthServer.Application.Common.Interfaces.Identity;
using AuthServer.Application.Common.Models.Settings.Application;
using AuthServer.Application.Common.Models.Settings.Email;
using AuthServer.Application.Models.Identity;
using AuthServer.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin",
                builder => builder.AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .SetIsOriginAllowed(origin => true)
                                  .AllowCredentials());
        });

        services.AddSignalR();

        services.AddHttpContextAccessor();

        services.AddSwaggerGen();

        services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileService, FileService>();
        services.Configure<ApplicationSettings>(configurationManager.GetSection(nameof(ApplicationSettings)));
        services.Configure<SmtpSettings>(configurationManager.GetSection(nameof(SmtpSettings)));
        services.Configure<JwtSettings>(configurationManager.GetSection(nameof(JwtSettings)));
        services.InitializeJwtTokenParameters(configurationManager);
        services.AddAuthorization();
        return services;
    }

    private static void InitializeJwtTokenParameters(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettingsSection = configuration.GetSection(nameof(JwtSettings));
        services.Configure<JwtSettings>(appSettingsSection);
        //// configure jwt authentication
        var jwtSettings = appSettingsSection.Get<JwtSettings>();

        var tokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Secret)),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"]!;
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            options.AccessType = "offline";
            options.SaveTokens = true;
        })
        .AddOAuth("LinkedIn", options =>
        {
            options.ClientId = configuration["Authentication:LinkedIn:ClientId"]!;
            options.ClientSecret = configuration["Authentication:LinkedIn:ClientSecret"]!;
            options.CallbackPath = new PathString("/signin-linkedin");
            options.AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization";
            options.TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
            options.UserInformationEndpoint = "https://api.linkedin.com/v2/me";

            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.SaveTokens = true;

            options.Events = new OAuthEvents
            {
                OnCreatingTicket = async context =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.linkedin.com/v2/userinfo");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                    var response = await context.Backchannel.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    using var user = JsonDocument.Parse(content);
                    context.RunClaimActions(user.RootElement);
                }
            };
        })
        .AddMicrosoftAccount(options =>
        {
            options.ClientId = configuration["Authentication:Microsoft:ClientId"]!;
            options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"]!;
            options.SaveTokens = true;
        })
        .AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = tokenValidationParameters;
            // SignalR token authentication
            x.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Headers.Authorization;
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/notificationHub"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Auth failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated");
                    return Task.CompletedTask;
                }
            };
        });
    }
}