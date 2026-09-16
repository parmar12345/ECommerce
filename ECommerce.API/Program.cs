using ECommerce.API.Middleware;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Application.Validators.Auth;
using ECommerce.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

const string CorsPolicyName = "AngularClient";

var builder = WebApplication.CreateBuilder(args);


// ==================================================
// 1. Controllers / Swagger
// ==================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ==================================================
// 2. Infrastructure
// ==================================================

builder.Services.AddInfrastructure(
    builder.Configuration);


// ==================================================
// 3. FluentValidation
// ==================================================

builder.Services.AddValidatorsFromAssemblyContaining<
    RegisterRequestValidator>();




// ==================================================
// 5. CORS
// ==================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// ==================================================
// 6. JWT Authentication
// ==================================================

var jwtSecret =
    builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException(
        "JWT SecretKey is not configured.");

var jwtIssuer =
    builder.Configuration["JwtSettings:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer is not configured.");

var jwtAudience =
    builder.Configuration["JwtSettings:Audience"]
    ?? throw new InvalidOperationException(
        "JWT Audience is not configured.");


builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSecret))
            };
    });

builder.Services.AddAuthorization();


// ==================================================
// 7. Rate Limiting
// ==================================================

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(
        "resend-verification",
        context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey:
                    context.Connection.RemoteIpAddress?
                        .ToString()
                    ?? "unknown",

                factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

    options.OnRejected = async (
        context,
        cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        context.HttpContext.Response.ContentType =
            "application/problem+json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                title = "Too many requests.",
                status = StatusCodes.Status429TooManyRequests,
                detail =
                    "Too many verification email requests. " +
                    "Please try again later.",
                instance =
                    context.HttpContext.Request.Path
            },
            cancellationToken);
    };
});


// ==================================================
// Build Application
// ==================================================

var app = builder.Build();


// ==================================================
// 8. Global Exception Middleware
// ==================================================

app.UseMiddleware<GlobalExceptionMiddleware>();


// ==================================================
// 9. Development Tools
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ==================================================
// 10. HTTP Pipeline
// ==================================================

app.UseHttpsRedirection();

app.UseRouting();

app.UseRateLimiter();

app.UseCors(CorsPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();


// ==================================================
// Run Application
// ==================================================

app.Run();