using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Application.Settings;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Persistence.Repositories;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Security;
using ECommerce.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==================================================
        // Database
        // ==================================================

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(
                    "DefaultConnection")));


        // ==================================================
        // Repositories
        // ==================================================

        services.AddScoped<
            IUserRepository,
            UserRepository>();

        services.AddScoped<
            IEmailVerificationTokenRepository,
            EmailVerificationTokenRepository>();

        services.AddScoped<
            IRefreshTokenRepository,
            RefreshTokenRepository>();

        services.AddScoped<
            IPasswordResetTokenRepository,
            PasswordResetTokenRepository>();

        services.AddScoped<
            IProductRepository,
            ProductRepository>();

        services.AddScoped<
            ICartRepository,
            CartRepository>();

        services.AddScoped<
            ICategoryRepository,
            CategoryRepository>();

        services.AddScoped<
            IProductImageRepository,
            ProductImageRepository>();

        services.AddScoped<
            IFileStorageService,
            LocalFileStorageService>();

        services.AddScoped<IAddressRepository, AddressRepository>();

        services.AddScoped<IAddressService, AddressService>();

           //umang

        // ==================================================
        // Application Services
        // ==================================================

        services.AddScoped<
            IAuthService,
            AuthService>();

        services.AddScoped<
            IEmailVerificationService,
            EmailVerificationService>();

        services.AddScoped<
            IProductService,
            ProductService>();

        services.AddScoped<
            ICategoryService,
            CategoryService>();

        services.AddScoped<
            ICartService,
            CartService>();


        // ==================================================
        // Infrastructure Services
        // ==================================================

        services.AddScoped<
            IEmailService,
            EmailService>();

        services.AddScoped<
            IPasswordHasher,
            BCryptPasswordHasher>();

        services.AddScoped<
            IJwtTokenService,
            JwtTokenService>();

        services.AddScoped<
            IRefreshTokenService,
            RefreshTokenService>();


        // ==================================================
        // Email Settings
        // ==================================================

        services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));


        return services;
    }
}