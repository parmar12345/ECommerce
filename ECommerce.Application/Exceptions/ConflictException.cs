using ECommerce.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Application.Exceptions;
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
