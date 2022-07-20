﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Core.Utilities.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _request;

        public ExceptionMiddleware(RequestDelegate request)
        {
            _request = request;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _request(context);
            }
            catch (Exception e)
            {
                await HandleAsync(context, e);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception e)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal server error";

            if (e.GetType() == typeof(ValidationException))
            {
                var errors = ((ValidationException)e).Errors;
                message = string.Join("\n", errors.Select(x => x.ErrorMessage));
            }

            await context.Response.WriteAsync(message);
        }
    }
}
