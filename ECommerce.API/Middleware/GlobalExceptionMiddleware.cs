using ECommerce.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ECommerce.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(
                context,
                ex);
        }
        catch (ConflictException ex)
        {
            await HandleConflictExceptionAsync(
                context,
                ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnauthorizedExceptionAsync(
                context,
                ex);
        }
        catch (InvalidOperationException ex)
        {
            await HandleBadRequestExceptionAsync(
                context,
                ex);
        }
        catch (Exception ex)
        {
            await HandleUnexpectedExceptionAsync(
                context,
                ex);
        }
    }

    // =========================================================
    // VALIDATION EXCEPTION - 400
    // =========================================================

    private async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        _logger.LogWarning(
            "Validation failed for {Path}",
            context.Request.Path);

        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(x => x.ErrorMessage)
                    .ToArray());

        var response = new
        {
            title = "Validation failed.",
            status = StatusCodes.Status400BadRequest,
            detail = "One or more validation errors occurred.",
            instance = context.Request.Path,
            errors = errors
        };

        context.Response.StatusCode =
            StatusCodes.Status400BadRequest;

        context.Response.ContentType =
            "application/problem+json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    // =========================================================
    // CONFLICT EXCEPTION - 409
    // =========================================================

    private async Task HandleConflictExceptionAsync(
        HttpContext context,
        ConflictException exception)
    {
        _logger.LogWarning(
            "Conflict: {Message}",
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict.",
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        await WriteProblemDetailsAsync(
            context,
            problemDetails);
    }

    // =========================================================
    // UNAUTHORIZED EXCEPTION - 401
    // =========================================================

    private async Task HandleUnauthorizedExceptionAsync(
        HttpContext context,
        UnauthorizedAccessException exception)
    {
        _logger.LogWarning(
            "Unauthorized request: {Message}",
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized.",
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        await WriteProblemDetailsAsync(
            context,
            problemDetails);
    }

    // =========================================================
    // BAD REQUEST - 400
    // =========================================================

    private async Task HandleBadRequestExceptionAsync(
        HttpContext context,
        InvalidOperationException exception)
    {
        _logger.LogWarning(
            "Bad request: {Message}",
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad request.",
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        await WriteProblemDetailsAsync(
            context,
            problemDetails);
    }

    // =========================================================
    // UNEXPECTED EXCEPTION - 500
    // =========================================================

    private async Task HandleUnexpectedExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred.");

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error.",
            Detail =
                "An unexpected error occurred while processing your request.",
            Instance = context.Request.Path
        };

        await WriteProblemDetailsAsync(
            context,
            problemDetails);
    }

    // =========================================================
    // WRITE RESPONSE
    // =========================================================

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        ProblemDetails problemDetails)
    {
        context.Response.StatusCode =
            problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        context.Response.ContentType =
            "application/problem+json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails));
    }
}