using EtaAnnouncer.Data;
using EtaAnnouncer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EtaAnnouncer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var passwordOptions = new PasswordOptions
            {
                RequireDigit = true,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequiredLength = 8
            };

            var devPasswordOptions = new PasswordOptions
            {
                RequiredLength = 3,
                RequireNonAlphanumeric = false,
                RequireLowercase = false,
                RequireUppercase = false,
                RequireDigit = false
            };

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password = passwordOptions;
#if DEBUG
                options.Password = devPasswordOptions;
#endif
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidAudience = builder.Configuration["Jwt:Audience"],
#if DEBUG
                    ValidateAudience = false
#endif
                };
            });

            builder.Services.AddControllers();
            builder.Services.AddScoped<IMapsService, MapsService>();
            builder.Services.AddSingleton(new TokenGeneratorService(builder.Configuration["Jwt:Key"]!,
                                                                    builder.Configuration["Jwt:Issuer"]!,
                                                                    builder.Configuration["Jwt:Audience"]));
            builder.Services.AddTransient<RefreshTokenService>();
            builder.Services.AddHostedService<TokenCleanupService>();
            builder.Services.AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var identityContext = services.GetRequiredService<IdentityDbContext>();
            identityContext.Database.Migrate();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
